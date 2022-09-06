using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 斐波那契Test
{
    public class CustomTask<T>
    {
        Queue<T>[] Ts;
        public CustomTask(int queCount)
        {
            if(queCount <= 0)
            {
                throw new ArgumentException("queue count error ");
            }
            Ts = new Queue<T>[queCount];
            for(int i = 0; i < queCount; i++)
            {
                Ts[i] = new Queue<T>();
            }
        }

        public Task InQueue(T[] datas)
        {           
            if (datas.Length <= 0)
            {
                throw new ArgumentException("count error ");
            }
            if(datas.Length < Ts.Length)
            {
                throw new ArgumentException($"length not less than {Ts.Length} ");
            }
            var count = Ts.Length;
            for (int i = 0; i < datas.Length; i++)
            {
                var data = datas[i];               
                Ts[i % count].Enqueue(data);
                Console.WriteLine($"已入栈 :{data} ,栈编号: {i%count}");
            }
            return Task.CompletedTask;
        }

        public async Task Run()
        {
            List<Task> tasks = new List<Task>();
            var count = Ts.Length-1;
            while (count >=0)
            {
                var task =Task.Factory.StartNew((pars) =>
                {
                    int queueNumber =int.Parse(pars.ToString()); 
                    while (Ts[queueNumber].TryDequeue(out var result))
                    {
                        Console.WriteLine($"已出栈: {result} 栈编号:{queueNumber}");
                    }
                }, count);
                tasks.Add(task);
                count--;              
            }
            await Task.WhenAll(tasks);
        }
    }
}
