using Akka.Actor;
using Akka.Configuration;
using Common.Library;
using System;

namespace AkkaClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var config = ConfigurationFactory.ParseString(@"
                                                        akka {  
                                                            actor {
                                                                provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
                                                            }
                                                            remote {
                                                                helios.tcp {
                                                                    transport-class = ""Akka.Remote.Transport.Helios.HeliosTcpTransport, Akka.Remote""
                                                                    applied-adapters = []
                                                                    transport-protocol = tcp
                                                                    port = 0
                                                                    hostname = localhost
                                                                }
                                                            }
                                                        }");
            using (var system = ActorSystem.Create("MyClient", config))
            {
                var greeting = system.ActorSelection("akka.tcp://MyServer@localhost:12345/user/Greeting");
                while (true)
                {
                    var input = Console.ReadLine();
                    if (input != "exit")
                    {
                        var id = Math.Abs(Guid.NewGuid().GetHashCode());
                        greeting.Tell(new JasonMessage() { Id = id, Name = $"{DateTime.Now} {input} " });
                    }
                    else
                    {
                        break;
                    }

                }
            }
            Console.WriteLine("Hello World!");
        }
    }
}
