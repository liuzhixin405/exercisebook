using LiveScore_ES.Backend.ReadModel;
using LiveScore_ES.Config;
using LiveScore_ES.Framework;
using Raven.Client.Documents.Session;

namespace LiveScore_ES.Backend.DAL
{
    public class EventRepository
    {
        private IDocumentSession DocumentSession { get; set; }
        public EventRepository()
        {
            DocumentSession = RavenDbConfig.Store.OpenSession();
        }

        public EventRepository Save(DomainEvent domainEvent)
        {
            // We can do this from within a start command; or here if we start with an event.
            var history = GetHistory(domainEvent.SagaId);
            if (history == null)
                BeginHistory(domainEvent.SagaId);

            DocumentSession.Store(new EventWrapper(domainEvent));
            var key = DocumentSession.Advanced.GetDocumentId(domainEvent);
            GetHistory(domainEvent.SagaId).Records.Add(key);
            return this;
        }

        //public void RecordEvent(String id, String eventName)
        //{
        //    var newEvent = new StateChangeEvent(id, eventName);
        //    DocumentSession.Store(newEvent);
        //    var key = DocumentSession.Advanced.GetDocumentId(newEvent); 
        //    GetHistory(id).Records.Add(key);
        //}

        public MatchHistory BeginHistory(String matchId)
        {
            var history = new MatchHistory(matchId);
            DocumentSession.Store(history);
            DocumentSession.SaveChanges();
            return history;
        }

        public MatchHistory GetHistory(String matchId)
        {
            //// Checks for existence without loading the document
            //var metadata = DocumentSession.Advanced.DocumentStore.DatabaseCommands.Head(matchId);
            //if (metadata == null)
            //    return null;

            return DocumentSession.Load<MatchHistory>(matchId);
        }

        public void UndoLastAction(String id)
        {
            //var lastEvent = DocumentSession.Query<StateChangeEvent>()
            //    .OrderByDescending(e => e.Timestamp)
            //    .FirstOrDefault(m => m.MatchId == id);

            var lastEvent = (from e in GetEventStreamFor(id) select e).LastOrDefault();
            if (lastEvent != null)
            {
                var key = DocumentSession.Advanced.GetDocumentId(lastEvent);
                DocumentSession.Delete(lastEvent);
                GetHistory(id).Records.Remove(key);
            }
        }

        public IList<DomainEvent> GetEventStreamFor(String id)
        {
            var history = GetHistory(id);
            if (history == null)
                return new List<DomainEvent>();

            var keys = history.Records;
            var list = DocumentSession.Load<EventWrapper>(keys);
            return list.Values.Select(x=>x.TheEvent).ToList();

            //return DocumentSession
            //    .Query<StateChangeEvent>()
            //    .Where(t => t.MatchId == id)
            //    .OrderBy(t => t.Timestamp);
        }

        public void Commit()
        {
            if (DocumentSession == null)
                return;
            DocumentSession.SaveChanges();
            DocumentSession.Dispose();
        }

        public void Empty(String id)
        {
            var records = GetEventStreamFor(id);
            foreach (var r in records)
            {
                if (r != null)
                    DocumentSession.Delete(r);
            }
            var history = GetHistory(id);
            if (history != null)
                DocumentSession.Delete(history);
            DocumentSession.SaveChanges();

            //RavenDbConfig.Instance.DatabaseCommands.DeleteByIndex(
            //    RavenDbConfig.MyDefaultIndex, new IndexQuery());
        }
    }
}
