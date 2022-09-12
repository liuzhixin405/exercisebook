using System.Net;

namespace HttpListenerSimple
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Listener(new string[] { "http://+:8080/" ,"http://+:8088/"});
            Console.ReadKey();
        }

        static void Listener(string[] prefixes)
        {
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("not support this os system");
                return;
            }
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");

            HttpListener httpListener = new HttpListener();
            foreach (var prefix in prefixes)
            {
                httpListener.Prefixes.Add(prefix);
            }
            httpListener.Start();
            Console.WriteLine("Listening...");
            HttpListenerContext context = httpListener.GetContext();
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            string responseString = "<HTML><BODY> Hello world!</BODY></HTML>";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
            httpListener.Close();
        }
    }


}