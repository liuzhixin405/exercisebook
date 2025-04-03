
using MinHashSharp;
using System.Reflection;
using System.Text.Json;

namespace DedupDemo
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://twitter241.p.rapidapi.com/user-tweets?user=1862413586087518208&count=1"),
                Headers =
    {
        { "x-rapidapi-key", "6e06920010mshb42b587b480d8c5p1e2bacjsn06ad280dbb86" },
        { "x-rapidapi-host", "twitter241.p.rapidapi.com" },
    },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                Console.WriteLine(body);

            }
        }
    }
    public class Tweet
    {
        public string full_text { get; set; }
    }

    public class ResponseModel
    {
        public List<Tweet> data { get; set; }
    }

}
