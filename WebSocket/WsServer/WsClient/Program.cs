using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WsClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            Console.WriteLine("press enter to continue...");
            Console.ReadLine();

            using (ClientWebSocket client = new ClientWebSocket())
            {
                Uri server = new Uri("ws://localhost:5000/send");
                var cts = new CancellationTokenSource();
                cts.CancelAfter(TimeSpan.FromSeconds(120));

                try
                {
                    await client.ConnectAsync(server, cts.Token);
                    var n = 0;
                    while (client.State.Equals(WebSocketState.Open))
                    {
                        Console.WriteLine("enter message to send");
                        string message = Console.ReadLine();
                        if (!string.IsNullOrEmpty(message))
                        {
                            ArraySegment<byte> byteToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
                            await client.SendAsync(byteToSend, WebSocketMessageType.Text, true, cts.Token);
                            var responseBuffer = new byte[1024];
                            var offset = 0;
                            var packet = 1024;
                            while (true)
                            {
                                ArraySegment<byte> byteRecieved = new ArraySegment<byte>(responseBuffer,offset,packet);
                                WebSocketReceiveResult response = await client.ReceiveAsync(byteRecieved, cts.Token);
                                var responseMessage = Encoding.UTF8.GetString(responseBuffer,offset,response.Count);
                                Console.WriteLine(responseMessage);
                                if (response.EndOfMessage)
                                    break;
                            }
                        }
                    }
                }
                catch (WebSocketException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            Console.ReadLine();
        }
    }
}
