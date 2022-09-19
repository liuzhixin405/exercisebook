using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ConsoleApp
{
    internal class Program
    {
        #region MyRegion
        //   static void Main(string[] args)
        //   {
        //       var throwIfNegative = new ActionBlock<int>(n =>
        //       {
        //           Console.WriteLine("n={0}", n);
        //           if (n < 0)
        //           {
        //               throw new ArgumentOutOfRangeException();
        //           }
        //       });
        //       throwIfNegative.Completion.ContinueWith((task) =>
        //       {
        //           Console.WriteLine("The status of the completion task is '{0}'.",
        //task.Status);

        //       });
        //       throwIfNegative.Post(0);
        //       throwIfNegative.Post(-1);
        //       throwIfNegative.Post(1);
        //       throwIfNegative.Post(-2);
        //       try
        //       {
        //           throwIfNegative.Completion.Wait();

        //       }
        //       catch (AggregateException ae)
        //       {
        //           ae.Handle(e =>
        //           {
        //               Console.WriteLine("Encountered {0}: {1}",
        //                  e.GetType().Name, e.Message);
        //               return true;
        //           });
        //       }
        //   } 
        #endregion
        static void Main(string[] args)
        {
            var bufferBlock = new BufferBlock<int>();

            for (int i = 0; i < 3; i++)
            {
                bufferBlock.Post(i);
            }
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine(bufferBlock.Receive());
            }
            var broadcastBlock = new BroadcastBlock<double>(null);
            broadcastBlock.Post(Math.PI);
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine(broadcastBlock.Receive());
            }

            var writeOnceBlock = new WriteOnceBlock<string>(null);

            Parallel.Invoke(
                           () => writeOnceBlock.Post("Message 1"),
                           () => writeOnceBlock.Post("Message 2"),
                           () => writeOnceBlock.Post("Message 3")
                           );
            Console.WriteLine(writeOnceBlock.Receive());
            Console.WriteLine(writeOnceBlock.Receive());

            var actionBlock = new ActionBlock<int>(n => Console.WriteLine(n));
            for (int i = 0; i < 3; i++)
            {
                actionBlock.Post(i * 10);
            }
            actionBlock.Complete();
            actionBlock.Completion.Wait();
            Console.WriteLine(actionBlock.Completion.Status);

            var transformBlock = new TransformBlock<int, double>(n => Math.Sqrt(n));
            transformBlock.Post(10);
            transformBlock.Post(20);
            transformBlock.Post(30);

            // Read the output messages from the block.
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine(transformBlock.Receive());
            }
            var transformManyBlock = new TransformManyBlock<string, char>(s => s.ToCharArray());

            transformManyBlock.Post("Hello");
            transformManyBlock.Post("World");

            for (int i = 0; i < ("Hello"+"World").Length; i++)
            {
                Console.WriteLine(transformManyBlock.Receive());
            }

            var batchBlock = new BatchBlock<int>(10);
            for (int i = 0; i < 13; i++)
            {
                batchBlock.Post(i);
            }
            batchBlock.Complete();

            Console.WriteLine("The sum of the elements in batch 1 is {0}.",
   batchBlock.Receive().Sum());

            Console.WriteLine("The sum of the elements in batch 2 is {0}.",
               batchBlock.Receive().Sum());

            var joinBlock = new JoinBlock<int, int, char>();
            joinBlock.Target1.Post(3);
            joinBlock.Target1.Post(6);
            joinBlock.Target2.Post(5);
            joinBlock.Target2.Post(4);

            joinBlock.Target3.Post('+');
            joinBlock.Target3.Post('-');

            for (int i = 0; i < 2; i++)
            {
                var data = joinBlock.Receive();
                switch (data.Item3)
                {
                    case '+':
                        Console.WriteLine("{0} + {1} = {2}",
                           data.Item1, data.Item2, data.Item1 + data.Item2);
                        break;
                    case '-':
                        Console.WriteLine("{0} - {1} = {2}",
                           data.Item1, data.Item2, data.Item1 - data.Item2);
                        break;
                    default:
                        Console.WriteLine("Unknown operator '{0}'.", data.Item3);
                        break;
                }
            }

            Func<int, int> DoWork = n =>
             {
                 if (n < 0)
                 {
                     throw new ArgumentOutOfRangeException();
                 }
                 return n;
             };
            var batchedJoinBlock = new BatchedJoinBlock<int, Exception>(7);

            foreach (int i in new int[] { 5, 6, -7, -22, 13, 55, 0 })
            {
                try
                {
                    // Post the result of the worker to the
                    // first target of the block.
                    batchedJoinBlock.Target1.Post(DoWork(i));
                }
                catch (ArgumentOutOfRangeException e)
                {
                    // If an error occurred, post the Exception to the
                    // second target of the block.
                    batchedJoinBlock.Target2.Post(e);
                }
            }
            var results = batchedJoinBlock.Receive();
            foreach (var item in results.Item1)
            {
                Console.WriteLine(item);
            }
            foreach (var item in results.Item2)
            {
                Console.WriteLine(item.Message);
            }

            Channel<string> channel = Channel.CreateBounded<string>(100);

            Task.Run(async () =>
            {
                for (int i = 0; i < 10; i++)
                {
                    await channel.Writer.WriteAsync($"{i}");
                }
            });

            Task.Run(async () => { 
            while(await channel.Reader.WaitToReadAsync())
                {
                    if(channel.Reader.TryRead(out var msg))
                    {
                        Console.WriteLine($"读取值为: {msg}");
                    }
                }
            
            });
            QueuePC();
            ProdComAsync().GetAwaiter().GetResult();
            
            Console.Read();

        }

        class MyProducer
        {
            private readonly ChannelWriter<int> _channelWriter;
            public MyProducer(ChannelWriter<int> channelWriter)
            {
                channelWriter = _channelWriter;
            }
        }
        class MyConsumer
        {
            private readonly ChannelReader<int> _channelReader;
            public MyConsumer(ChannelReader<int> channelReader)
            {
                _channelReader = channelReader;
            }
        }


        static async Task Main2(string[] args)
        {
            //var myChannel = Channel.CreateUnbounded<int>();
            //var producer = new MyProducer(myChannel.Writer);
            //var consumer = new MyConsumer(myChannel.Reader);


            var myChannel = Channel.CreateUnbounded<int>();

            _ = Task.Factory.StartNew(async () =>
            {
                for (int i = 0; i < 10; i++)
                {
                    await myChannel.Writer.WriteAsync(i);
                }

                myChannel.Writer.Complete();
            });

            try
            {
                while (true)
                {
                    var item = await myChannel.Reader.ReadAsync();
                    Console.WriteLine(item);
                    await Task.Delay(1000);
                }
            }
            catch (ChannelClosedException e)
            {
                Console.WriteLine("Channel was closed!");
            }
        }

        static async Task Main3(string[] args)
        {
            var myChannel = Channel.CreateUnbounded<int>();

            _ = Task.Factory.StartNew(async () =>
            {
                for (int i = 0; i < 10; i++)
                {
                    await myChannel.Writer.WriteAsync(i);
                }

                myChannel.Writer.Complete();
            });

            await foreach (var item in myChannel.Reader.ReadAllAsync())
            {
                Console.WriteLine(item);
                await Task.Delay(1000);
            }
        }
        static void Main4(string[] args)
        {
            //CreateBounded有大小限制  CreateUnbounded无大小限制
            /*
             背压(特别是当涉及消息传递/排队时)是指资源(无论是内存、ram、网络)是有限的。我们应该能够在链条上施加“压力”，试着减轻一些压力。至少，让生态系统中的其他人知道我们负荷过重，我们可能需要一些时间来处理他们的请求。

一般来说，当我们讨论队列的背压时。几乎所有情况下，我们都在讨论一种方法，告诉任何试图在队列中添加更多条目的人，要么他们根本无法再加入任何条目，要么他们需要推后一段时间。更罕见的是，我们讨论的队列是在达到一定容量时纯粹丢弃消息。这种情况很少发生，但是我们有这个选项。
             */
            var channelOptions = new BoundedChannelOptions(5)
            {
                FullMode = BoundedChannelFullMode.Wait
            };

            var myChannel = Channel.CreateBounded<int>(channelOptions);
        }
        #region queue
        /// <summary>
        /// 线程安全的顺序不可变
        /// </summary>
        /// <returns></returns>
        static async Task ProdComAsync()
        {
            var myChannel = Channel.CreateUnbounded<int>();
            Console.WriteLine("channel表演开始:");
            _ = Task.Factory.StartNew(async () =>
            {
                for (int i = 0; i < 10; i++)
                {
                    await myChannel.Writer.WriteAsync(i);
                    await Task.Delay(1000);
                }
            });

            while (true)
            {
                var item = await myChannel.Reader.ReadAsync();
                Console.WriteLine(item);
            }
        }

        static void QueuePC()
        {
            Console.WriteLine("queue表演开始:");
            Queue queue = new System.Collections.Queue();

            Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < 10; i++)
                {
                    queue.Enqueue(i);
                }
            });
            Thread.Sleep(5000);
            while (true)
            {
                if (queue.Count > 0)
                {
                    var item = queue.Dequeue();

                    Console.WriteLine(item);
                }
                else
                {
                    break;
                }

            }

        } 
        #endregion
    }
}
