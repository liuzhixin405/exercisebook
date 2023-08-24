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
            //var actionBlock = new ActionBlock<int>(n => Console.WriteLine(n));

            //// Post several messages to the block.
            //for (int i = 0; i < 3; i++)
            //{
            //    actionBlock.Post(i * 10);
            //}

            //// Set the block to the completed state and wait for all
            //// tasks to finish.
            //actionBlock.Complete();
            //actionBlock.Completion.Wait();




            BufferBlock<Student> bufferBlock = new BufferBlock<Student>(new DataflowBlockOptions {  BoundedCapacity= 2});
            ActionBlock<Student> actionBlock = new ActionBlock<Student> ( data=>Run(data), new ExecutionDataflowBlockOptions { BoundedCapacity = 10, MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded } );
            bufferBlock.LinkTo( actionBlock );
            int i = 10;
            while (i != 0)
            {
                i--;
               await bufferBlock.SendAsync(new Student { Id = i ,Name = $"test{i}"});
            }
            Console.ReadKey();
        }

        static void Run(Student student)
        {
            Console.WriteLine($"Student :{student.Id} ，Name :{student.Name}");
        }
    } //使用Channel最佳
    internal class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Age { get; set; }
    }
}
