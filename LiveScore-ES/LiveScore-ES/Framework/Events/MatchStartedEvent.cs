namespace LiveScore_ES.Framework.Events
{
    public class MatchStartedEvent:DomainEvent
    {
        public MatchStartedEvent(string id)
        {
            MatchId = id;
            SagaId = id;
        }

        public string MatchId { get; private set; }
    }
}
