using Grpc.Core;
using GrpcGreeter;

namespace GrpcGreeter.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }

        public override Task<OrderResponse> GetOrderById(OrderRequest request, ServerCallContext context)
        {
            if (request.OrderId.Equals(123))
                //数据库获取order通过 request.OrderId;
                return Task.FromResult(new OrderResponse
                {
                    Json = System.Text.Json.JsonSerializer.Serialize("hello world")
                });
            else
                return null;
        }
    }
}