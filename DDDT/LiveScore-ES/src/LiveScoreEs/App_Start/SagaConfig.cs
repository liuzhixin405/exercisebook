using LiveScoreEs.Framework;
using LiveScoreEs.Framework.Commands;
using LiveScoreEs.Framework.Events;
using LiveScoreEs.Framework.Sagas;

namespace LiveScoreEs
{
    public class SagaConfig
    {
        public static void Initialize()
        {
            Bus.RegisterSaga<MatchStartedEvent, MatchSaga>();
        }
    }
}