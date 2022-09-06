using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace DataFlowDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var res = Invoke();
            Console.WriteLine(res);
            Console.Read();
        }

        async static Task<bool> Invoke()
        {
            var throwInfNegative = new ActionBlock<int>(n => {
                Console.WriteLine($"n={n}");
                if (n < 0)
                    throw new ArgumentOutOfRangeException();
            });
            throwInfNegative.Completion.ContinueWith(task =>
            {
                Console.WriteLine("The status of the completion task is '{0}'.",
                   task.Status);
            });
            throwInfNegative.Post(0);
            throwInfNegative.Post(-1);
            throwInfNegative.Post(1);
            throwInfNegative.Post(-2);
            throwInfNegative.Complete();
            try
            {
               throwInfNegative.Completion.Wait();
            }
            catch(AggregateException ex)
            {
                ex.Handle(e =>
                {
                    Console.WriteLine($"Encountered {ex.GetType().Name}:{ex.Message}");
                    return true;
                });
            }
            return true;
        }
    }
}
