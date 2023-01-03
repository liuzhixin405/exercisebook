using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            List<Task> tasks = new List<Task>();
            tasks.Add(Task.Run(async () =>
            {

                IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync("localhost");
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                var ipEndPoint = new IPEndPoint(ipAddress, 13);
                TcpListener listener = new(ipEndPoint);
                try
                {
                    listener.Start();
                    using TcpClient handler = await listener.AcceptTcpClientAsync();
                    await using NetworkStream stream = handler.GetStream();

                    int i = 10;
                    while (i > 0)
                    {
                        Console.WriteLine($"请输入第{11 - i}条要发送的消息:");
                        var message = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(message))
                        {
                            message = "默认消息";
                        }
                        var dateTimeBytes = Encoding.UTF8.GetBytes(message);
                        await stream.WriteAsync(dateTimeBytes);
                        Console.WriteLine($"Sent第{11-i}条 message: \"{message}\"");
                        i--;
                    }
                }
                finally
                {
                    listener.Stop();
                }

            }));
            tasks.Add(Task.Run(async () =>
            {

                IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync("localhost");
                IPAddress ipAddress = ipHostInfo.AddressList[0];

                IPEndPoint ipEndPoint = new(ipAddress, 13);

                using TcpClient client = new();
                await client.ConnectAsync(ipEndPoint);

                await using NetworkStream stream = client.GetStream();
                int i = 10;
                while (i > 0)
                {
                    var buffer = new byte[1_024];
                    int received = await stream.ReadAsync(buffer);

                    var message = Encoding.UTF8.GetString(buffer, 0, received);
                    Console.WriteLine($"Message第{11-i}条 received: \"{message}\"");
                    i--;
                }
                Console.Read();
            }));
            Task.WaitAll(tasks.ToArray());
            await Task.Delay(1000);
        }
    }
}