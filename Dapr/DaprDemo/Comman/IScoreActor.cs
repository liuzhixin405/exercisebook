using Dapr.Actors;
using Dapr.Actors.Runtime;

namespace Comman
{
    public interface IScoreActor:IActor
    {
        Task<int> IncrementScoreAsync();

        Task<int> GetScoreAsync();
    }

    
}