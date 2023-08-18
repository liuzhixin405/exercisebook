using Grpc.Core;
using Helloworld;

namespace grpc_service_web.Services;
public class HelloWorldService : Helloworld.Greeter.GreeterBase
{
    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        return Task.FromResult<HelloReply>(new HelloReply(){Message = $"coreqi {request.Name}" });
    }

    public override Task SayRepeatHello(RepeatHelloRequest request, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
    {
        return Task.FromResult<Task>(
            responseStream.WriteAsync(new HelloReply() { Message = $"coreqi {request.Name}" })
        );
    }
}
