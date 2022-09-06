using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace BufferBlockDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //var bufferBlock = new BufferBlock<int>();
            //for(int i =0; i< 3; i++)
            //{
            //   await bufferBlock.SendAsync(i);
            //}

            //for (int i = 0; i < 3; i++)
            //{
            //    Console.WriteLine(await bufferBlock.ReceiveAsync());
            //}


            // Create an ActionBlock<int> object that prints values
            // to the console.
            var actionBlock = new ActionBlock<int>(n => Console.WriteLine(n));

            // Post several messages to the block.
            for (int i = 0; i < 3; i++)
            {
                actionBlock.Post(i * 10);
            }

            // Set the block to the completed state and wait for all
            // tasks to finish.
            actionBlock.Complete();
            actionBlock.Completion.Wait();
            Console.ReadKey();
        }
    }
}
