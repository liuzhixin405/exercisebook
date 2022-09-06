using Akka.Actor;
using Akka.Configuration;
using Common.Library;
using System;

namespace ServerAkka
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
                                                                    port = 51179
                                                                    hostname = localhost
                                                                }
                                                            }
                                                        }");
            using (var system = ActorSystem.Create("MyServer", config))
            {
                system.ActorOf<JasonActor>("Greeting");

                Console.ReadLine();
            }

            Console.WriteLine("Hello World!");
        }
    }

    public class JasonActor : UntypedActor
    {
        protected override void OnReceive(object greet)
        {
            var greet1 = (JasonMessage)greet;
            Console.WriteLine($"当前时间：{DateTime.Now.Ticks}, Name:{greet1.Name}, Id:{greet1.Id} ");
        }
    }

}