using Comman;
using Dapr.Actors;
using Dapr.Actors.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ScoreController : ControllerBase
    {
        private readonly IActorProxyFactory _actorProxyFactory;
        public ScoreController(IActorProxyFactory actorProxyFactory)
        {

            _actorProxyFactory = actorProxyFactory;

        }

        [HttpGet]
        [Route("increment")]
        public Task<int> IncrementAsync(string scoreId)
        {
            var scoreActor = _actorProxyFactory.CreateActorProxy<IScoreActor>(
                new ActorId(scoreId),
                "ScoreActor");

            return scoreActor.IncrementScoreAsync();
        }
    }
}
