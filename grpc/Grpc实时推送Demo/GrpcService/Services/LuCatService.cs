using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace GrpcService.Services
{
    public class LuCatService:LuCat.LuCatBase
    {
        private static readonly List<string> Cats = new List<string>() { "英短银渐层", "英短金渐层", "美短", "蓝猫", "狸花猫", "橘猫" };
        private static readonly Random Rand = Random.Shared;

        private readonly ILogger<LuCatService> _logger;
        public LuCatService(ILogger<LuCatService> logger)
        {
            _logger = logger;
        }
        public override async Task BathTheCat(IAsyncStreamReader<BathTheCatReq> requestStream, IServerStreamWriter<BathTheCatResp> responseStream, ServerCallContext context)
        {
            var batchQueue = new Queue<int>();
            while(!context.CancellationToken.IsCancellationRequested && await requestStream.MoveNext())
            {
                //将要洗澡的猫加入队列
                batchQueue.Enqueue(requestStream.Current.Id);
                _logger.LogInformation($"Cat {requestStream.Current.Id} Enqueue.");
            }
            while(batchQueue.TryDequeue(out var catId))
            {
                await responseStream.WriteAsync(new BathTheCatResp { Message = $"铲屎的成功给一只{Cats[catId]}洗了澡！" });
                await Task.Delay(500);//此处主要是为了方便客户端能看出流调用的效果
            }
        }

        public override Task<CountCatResult> Count(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new CountCatResult { Count=Cats.Count });
        }

        public override Task<SuckingCatResult> SuckingCat(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new SuckingCatResult()
            {
                Message = $"您吸了一只{Cats[Rand.Next(0, Cats.Count)]}"
            });
        }


    }
}
