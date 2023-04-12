// See https://aka.ms/new-console-template for more information
using System;
using System.Threading;
using System.Threading.Tasks;

CancellationTokenSource tokenSource = new CancellationTokenSource();
Task.Run(() => {
    Thread.Sleep(2000);
    tokenSource.Cancel();
});

try
{
    //var size = DownloadFile(tokenSource.Token);
    var size = DownloadFileNext(tokenSource.Token);
    Console.WriteLine($"文件大小:{size}");
}
catch(OperationCanceledException ex)
{
    Console.WriteLine($"文件下载失败: {ex.Message}");

}
finally
{
    tokenSource.Dispose();
}

Thread.Sleep(2000);
Console.Read();


int DownloadFile(CancellationToken token)
{
    token.Register(() => {
        Console.WriteLine("监听到取消事件");
    
    });

    Console.WriteLine("开始下载文件");

    for (int i = 0; i < 10; i++)
    {
        token.ThrowIfCancellationRequested();
        Console.WriteLine(i.ToString());
        Thread.Sleep(300);
    }
    Console.WriteLine("文件下载完成");
    return 100;
}

int DownloadFileNext(CancellationToken externalToken)
{
    var timeOutToken = new CancellationTokenSource(new TimeSpan(0, 0, 1)).Token;
    using(var linkToken = CancellationTokenSource.CreateLinkedTokenSource(externalToken, timeOutToken))
    {
        Console.WriteLine("开始下载文件");
        for (int i = 0; i < 5; i++)
        {
            linkToken.Token.ThrowIfCancellationRequested();
            Console.WriteLine(i.ToString());
            Thread.Sleep(400);
        }
        Console.WriteLine("文件下载完成");
        return 100;
    }
}