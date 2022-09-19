using System.Collections.Concurrent;

namespace Next
{

    internal class Program
    {

        static void Main(string[] args)
        {
            var channel = new Channel<string>();
            //Task.WaitAll(Producer(channel), Consumer(channel));

            //Task.WaitAll( Consumer(channel),Producer(channel));
            Task.WaitAll(Consumer(channel), Producer(channel), Producer(channel), Producer(channel)); // 这里会生产三个消费三个
        }
        static async Task Producer(IWrite<string> write)
        {
            for (int i = 0; i < 100; i++)
            {
                write.Push(i.ToString());
                Console.WriteLine($"生产=>{i}");
                await Task.Delay(100);
            }
            write.Complete();
        }

        static async Task Consumer(IRead<string> reader)
        {
            while (!reader.IsComplete())
            {
                var msg = await reader.Read();
                Console.WriteLine($"消费=>{msg}");
            }
        }
    }
}



public interface IRead<T>
{
    Task<T> Read();
    bool IsComplete();
}

public interface IWrite<T>
{
    void Push(T msg);
    void Complete();
}

public class Channel<T> : IRead<T>, IWrite<T>
{
    private bool finished;
    private ConcurrentQueue<T> queue;
    private SemaphoreSlim flag;
    public Channel()
    {
        queue = new();
        flag = new SemaphoreSlim(0);
    }

    public void Complete()
    {
        finished = true;
        Console.WriteLine("is complete");
    }

    public bool IsComplete()
    {
        return finished;
    }

    public void Push(T msg)
    {
        queue.Enqueue(msg);
        flag.Release();
    }

    public async Task<T> Read()
    {
        await flag.WaitAsync();
        if (queue.TryDequeue(out var msg))
            return msg;
        return default(T);
    }
}