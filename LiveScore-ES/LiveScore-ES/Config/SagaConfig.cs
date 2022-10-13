using LiveScore_ES.Framework;
using LiveScore_ES.Framework.Events;
using LiveScore_ES.Framework.Sagas;

namespace LiveScore_ES.Config
{
    public class SagaConfig
    {
        public static void Initialize()
        {
            Bus.RegistorSaga<MatchStartedEvent, MatchSaga>();
        }
    }
}
