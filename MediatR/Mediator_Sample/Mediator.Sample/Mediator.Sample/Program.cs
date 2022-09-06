using Mediator.Sample.Handler;
using Mediator.Sample.Model;
using MediatR;
using Microsoft.Extensions.Hosting;

namespace Mediator.Sample;

internal class Program
{

    static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        //builder.Services.AddControllers();
        builder.Services.AddMediatR(typeof(Program).Assembly);
        builder.Services.AddScoped(typeof(IStreamPipelineBehavior<StreamPing,Pong>),typeof(MyBehaviorDo));
        var serviceProvider = builder.Services.BuildServiceProvider();
        var _mediator = serviceProvider.CreateAsyncScope().ServiceProvider.GetRequiredService<IMediator>(); 
        var app = builder.Build();
        app.Use(async (context, next) =>
        {
            await _mediator.Send(new NoParameter());
            await _mediator.Publish(new PData { Message = " 来自publish的问候" });
            Console.WriteLine("*************************");
            var result = await _mediator.Send(new PingSource { Message = "你好 ,pars" });
            Console.WriteLine(result.Message); 
            Console.WriteLine("*************************");

           var smessages =  _mediator.CreateStream(new StreamPing { StreamMessage = "stream 消息" });
           await foreach (var pong in smessages)
            {
                Console.WriteLine(pong.Message);
            }
            Console.WriteLine("*************************");

            await next(context);
        });
        //app.MapControllers();
        app.Run();
    }



}