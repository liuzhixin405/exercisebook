using Newtonsoft.Json;
using StandardLibrary.AttributeExt;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace StandardLibrary.HttpHelper
{
    public class RequestApi
    {
        public virtual string InvokeApi(string url,string parmeters = null)
        {
            using(HttpClient client = new HttpClient())
            {
                HttpRequestMessage message = new HttpRequestMessage();
                message.Method = HttpMethod.Get;
                message.RequestUri = new Uri(url);

                var result = client.SendAsync(message).Result;
                if (result.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception(JsonConvert.SerializeObject(result.RequestMessage));

                }

                string content = result.Content.ReadAsStringAsync().Result;
                return content;
            }
        }

        [CustomPollyFallbackAttribute]
        [CustomPollyRetryAttribute]
        public virtual async Task<string> InvokeApiAsync(string url, string parmeters = null)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage message = new HttpRequestMessage();
                message.Method = HttpMethod.Get;
                message.RequestUri = new Uri(url);

                var result = await client.SendAsync(message);
                if (result.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception(JsonConvert.SerializeObject(result.RequestMessage));

                }

                string content = await result.Content.ReadAsStringAsync();
                return content;
            }
        }
    }
}
