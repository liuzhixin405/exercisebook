using Akka.Actor;
using System;
using System.Threading.Tasks;

namespace ActorDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var system = ActorSystem.Create("test");
            var greeter = system.ActorOf<JasonActor>("jason");
            for (int i = 0; i < 100; i++)
            {
                Task.Run(() =>
                {
                    var id = Math.Abs(Guid.NewGuid().GetHashCode());
                    greeter.Tell(new JasonMessage { Id=id,Name= $"{DateTime.Now.Ticks} {id}  {i}  " });
                });
            }
            Console.Read();
        }
    }

    internal class JasonActor : ReceiveActor
    {
        public JasonActor()
        {
            Receive<JasonMessage>(greet =>
            {
                Console.WriteLine($"当前时间：{DateTime.Now.Ticks}, Name:{greet.Name}, Id:{greet.Id} ");
            });
        }
    }

    internal class JasonMessage
    {
        internal long Id { get; set; }
        internal string Name { get; set; }
    }
}
