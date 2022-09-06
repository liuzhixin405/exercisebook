using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace KafkaClient
{
    public class ConfulentKafka
    {
        public static void Run_Consume(string brokerList,List<string> topics,string group)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = brokerList,
                GroupId = group,
                EnableAutoCommit = false,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnablePartitionEof = true,
                PartitionAssignmentStrategy = PartitionAssignmentStrategy.Range,
                SessionTimeoutMs = 6000,
                MaxPollIntervalMs = 10000
            };

            const int commitPeriod = 1;

            using(var consumer = new ConsumerBuilder<Ignore,string>(config)
                .SetErrorHandler((_,e)=> Console.WriteLine($"Error: {e.Reason}")).SetPartitionsAssignedHandler
                ((c,partitions)=> {
                    Console.WriteLine($"Assigned partitions: [{string.Join(", ", partitions)}]");
                })
                .SetPartitionsRevokedHandler((c,partitions)=> {
                    Console.WriteLine($"Revoking assignment: [{string.Join(", ", partitions)}]");
                }).Build()
                )
            {
                consumer.Subscribe(topics);

                try
                {
                    while (true)
                    {
                        try
                        {
                            var consumeResult = consumer.Consume();
                            if (consumeResult.IsPartitionEOF)
                            {
                                continue;
                            }

                            Console.WriteLine($": {consumeResult.TopicPartitionOffset}::{consumeResult.Message.Value}");

                            if(consumeResult.Offset % commitPeriod == 0)
                            {
                                try
                                {
                                    Thread.Sleep(1000);
                                    consumer.Commit(consumeResult);
                                }
                                catch
                                {

                                }
                            }
                        }
                        catch(Exception ex)
                        {

							Console.WriteLine(ex.Message);
							throw;
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("Closing consumer.");
                    consumer.Close();
                }
            }
        }

		public static void Run_ManualAssign(string brokerList, List<string> topics, CancellationToken cancellationToken)
		{
			var config = new ConsumerConfig
			{
				// the group.id property must be specified when creating a consumer, even 
				// if you do not intend to use any consumer group functionality.
				GroupId = new Guid().ToString(),
				BootstrapServers = brokerList,
				// partition offsets can be committed to a group even by consumers not
				// subscribed to the group. in this example, auto commit is disabled
				// to prevent this from occurring.
				EnableAutoCommit = true
			};

			using (var consumer =
				new ConsumerBuilder<Ignore, string>(config)
					.SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}")).SetPartitionsAssignedHandler((c, partitions) =>
					{
						Console.WriteLine($"Assigned partitions: [{string.Join(", ", partitions)}]");
					})
					.Build())
			{
				List<TopicPartitionOffset> topicsss = new List<TopicPartitionOffset>();
				consumer.Assign(topics.Select(topic => new TopicPartitionOffset(topic, 1, Offset.Beginning)).ToList());
				try
				{
					while (true)
					{
						try
						{
							var consumeResult = consumer.Consume(cancellationToken);
							Console.WriteLine($"Received message at {consumeResult.TopicPartitionOffset}: ${consumeResult.Message.Value}");
						}
						catch (ConsumeException e)
						{
							Console.WriteLine($"Consume error: {e.Error.Reason}");
						}
					}
				}
				catch (OperationCanceledException)
				{
					Console.WriteLine("Closing consumer.");
					consumer.Close();
				}
			}
		}

		private static void PrintUsage()
			=> Console.WriteLine("Usage: .. <subscribe|manual> <broker,broker,..> <topic> [topic..]");

		public static void Consumer(string brokerlist, List<string> topicname, string groupname)
		{

			var mode = "subscribe";
			var brokerList = brokerlist;
			var topics = topicname;
			Console.WriteLine($"Started consumer, Ctrl-C to stop consuming");
			CancellationTokenSource cts = new CancellationTokenSource();
			Console.CancelKeyPress += (_, e) =>
			{
				e.Cancel = true; // prevent the process from terminating.
				cts.Cancel();
			};

			switch (mode)
			{
				case "subscribe":
					Run_Consume(brokerList, topics, groupname);
					break;
				case "manual":
					Run_ManualAssign(brokerList, topics, cts.Token);
					break;
				default:
					PrintUsage();
					break;
			}
		}
	}
}
