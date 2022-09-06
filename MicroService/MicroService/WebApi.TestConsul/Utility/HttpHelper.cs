using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace WebApi.TestConsul.Utility
{
    public class HttpHelper
    {
        /// <summary>
        /// get请求core webapi
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string InvokeApi(string url)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpRequestMessage message = new HttpRequestMessage();
                message.Method = HttpMethod.Get;
                message.RequestUri = new Uri(url);
                var result = httpClient.SendAsync(message).Result;
                string content = result.Content.ReadAsStringAsync().Result;
                return content;
            }
        }
    }
}
