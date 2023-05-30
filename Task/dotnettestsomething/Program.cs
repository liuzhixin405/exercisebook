/* Task.Factory.StartNew(Run,TaskCreationOptions.LongRunning);
Console.Read();

void Run(){
    while(true){
        Do();
    }
}
void Do(){
    var end = DateTime.UtcNow.AddSeconds(2);
    SpinWait.SpinUntil(()=>DateTimeOffset.UtcNow>end);
    var isThreadPoolThread = Thread.CurrentThread.IsThreadPoolThread;
    System.Console.WriteLine($"[{DateTimeOffset.Now}] is thread pool thread: {isThreadPoolThread}");
}
 */ //同步操作  加TaskCreationOptions.LongRunning非线程池线程


/* Task.Factory.StartNew(RunAsync,TaskCreationOptions.LongRunning);
Console.Read();

async Task RunAsync(){
    while(true){
        DoAsync();
    }
}
async Task DoAsync(){
     await Task.Delay(2000);
    var isThreadPoolThread = Thread.CurrentThread.IsThreadPoolThread;
    System.Console.WriteLine($"[{DateTimeOffset.Now}] is thread pool thread: {isThreadPoolThread}");
}
 */   //异步操作  加TaskCreationOptions.LongRunning依然是线程池线程

/* 
Task.Factory.StartNew(() => { while (true) Do(); }, TaskCreationOptions.LongRunning);
Console.Read();

void  Do()
{
    Task.Delay(2000).Wait();
    var isThreadPoolThread = Thread.CurrentThread.IsThreadPoolThread;
    Console.WriteLine($"[{DateTimeOffset.Now}]Is thread pool thread: {isThreadPoolThread}");
}  */ //  加TaskCreationOptions.LongRunning是线程池线程 这是wait后的同步操作


/* Task.Factory.StartNew(RunAsync,CancellationToken.None,TaskCreationOptions.LongRunning,new DedicatedThreadTaskScheduler(1));
System.Console.Read();

async Task RunAsync(){
    while(true){
        await DoAsync();
    }
}

async Task DoAsync(){
    await Task.Delay(2000);
    var isThreadPoolThread = Thread.CurrentThread.IsThreadPoolThread;
      Console.WriteLine($"[{DateTimeOffset.Now}]Is thread pool thread: {isThreadPoolThread}");
}   //这样又可以了 */

Task.Factory.StartNew(()=>Task.WhenAll(Enumerable.Range(1,6).Select(it=>DoAsync(it))),CancellationToken.None,
TaskCreationOptions.None,
new DedicatedThreadTaskScheduler(2));

async Task DoAsync(int index){
    await Task.Yield();
    System.Console.WriteLine($"[{DateTimeOffset.Now.ToString("hh:MM:ss")}]Task {index} is executed in thread {Environment.CurrentManagedThreadId}");

    var endTime = DateTime.UtcNow.AddSeconds(4);
    SpinWait.SpinUntil(()=>DateTime.UtcNow>endTime);
    await Task.Delay(1000);
}

Console.ReadLine();