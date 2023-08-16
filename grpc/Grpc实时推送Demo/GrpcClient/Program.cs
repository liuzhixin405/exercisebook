
using Grpc.Net.Client;
using GrpcService;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

var cts = new CancellationTokenSource();
cts.CancelAfter(TimeSpan.FromSeconds(8));
using var channel = GrpcChannel.ForAddress("https://localhost:7051");


//var client = new Greeter.GreeterClient(channel);
//var reply = await client.SayHelloAsync(new HelloRequest { Name = "GreeterClient" });
//Console.WriteLine($"Greeting:  {reply.Message}");

var catClient = new LuCat.LuCatClient(channel);
var catReply = await catClient.SuckingCatAsync(new Empty());
Console.WriteLine("调用撸猫服务：" + catReply.Message);


//获取猫总数
var catCount = await catClient.CountAsync(new Empty());
Console.WriteLine($"一共{catCount.Count}只猫。");
var rand = new Random(DateTime.Now.Millisecond);

var bathCat = catClient.BathTheCat(cancellationToken:cts.Token);

//定义接收吸猫响应逻辑
var bathCatRespTask = Task.Run(async () =>
{
    try
    {
        await foreach (var resp in bathCat.ResponseStream.ReadAllAsync())
        {
            Console.WriteLine(resp.Message);
        }
    }
    catch (RpcException ex) when (ex.StatusCode ==StatusCode.Cancelled)
    {
        Console.WriteLine("Stream cancelled.");
    }
   
});
//随机给10个猫洗澡
for (int i = 0; i < 10; i++)
{
    //await Task.Delay(1000);
    await bathCat.RequestStream.WriteAsync(new BathTheCatReq() { Id = rand.Next(0, catCount.Count) });
}
//发送完毕
await bathCat.RequestStream.CompleteAsync();
Console.WriteLine("客户端已发送完10个需要洗澡的猫id");
Console.WriteLine("接收洗澡结果：");
//开始接收响应
await bathCatRespTask;

Console.WriteLine("Press any key to exit...");
Console.ReadKey();