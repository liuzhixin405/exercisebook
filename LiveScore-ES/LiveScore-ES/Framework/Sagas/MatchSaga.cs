using LiveScore_ES.Backend.DAL;
using LiveScore_ES.Framework.Commands;
using LiveScore_ES.Framework.Events;

namespace LiveScore_ES.Framework.Sagas
{
    public class MatchSaga:SagaBase<MatchData>,IStartWithMessage<MatchStartedEvent>,ICanhandleMessage<EndMatchCommand>
    {
        private readonly EventRepository _repo = new EventRepository();
        public void Handle(MatchStartedEvent message)
        {
            _repo.BeginHistory(message.MatchId);
            _repo.Save(message).Commit();

            SagaId = message.MatchId;
        }

        public void Handle(EndMatchCommand message)
        {

        }
    }
}
