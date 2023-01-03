using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TCPTest
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync("localhost");
            IPAddress ipAddress = ipHostInfo.AddressList[0];

            IPEndPoint ipEndPoint = new(ipAddress, 13);

            using TcpClient client = new();
            await client.ConnectAsync(ipEndPoint);

            await using NetworkStream stream = client.GetStream();

            var buffer = new byte[1_024];
            int received = await stream.ReadAsync(buffer);

            var message = Encoding.UTF8.GetString(buffer, 0, received);
            Console.WriteLine($"Message received: \"{message}\"");
            Console.Read();
        }
    }
}