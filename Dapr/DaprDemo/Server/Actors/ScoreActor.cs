using Comman;
using Dapr.Actors.Runtime;
using System.Text.Json;
using System.Text;

namespace Server.Actors
{
    public class ScoreActor : Actor, IScoreActor
    {
        public ScoreActor(ActorHost host) : base(host)
        {
        }

        public async Task<int> GetScoreAsync()
        {
            var scoreValue = await StateManager.TryGetStateAsync<int>("score");
            if (scoreValue.HasValue)
            {
                return scoreValue.Value;
            }

            return 0;
        }

        public Task<int> IncrementScoreAsync()
        {
            return StateManager.AddOrUpdateStateAsync("score", 1, (key, currentScore) => currentScore + 1);
        }
    }
}
