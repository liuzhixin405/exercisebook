using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.Utility.HttpHelper
{
    public class HttpClientHelper
    {
        public async Task<string> PostResponseAsync(HttpClient httpClient, string url, string postData, IDictionary<string, string> dic)
        {
            HttpContent httpContent = new StringContent(postData);
            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            httpContent.Headers.ContentType.CharSet = "utf-8";

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", dic["Authorization"]);

            var response = await httpClient.PostAsync(url, httpContent);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                return await Task.FromResult($"e!r!r!o!r: {response.StatusCode}");
            }
        }


        public async Task<string> GetResponseAsync(HttpClient httpClient, string url, Dictionary<string, string> dic)
        {
            foreach (var item in dic)
            {
                httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
            }
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            var response = await httpClient.GetAsync(url);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                return await Task.FromResult($"e!r!r!o!r: {response.StatusCode}");
            }
        }

        // Put请求
        public string PutResponse(string url, string putData, out string statusCode)
        {
            string result = string.Empty;
            HttpContent httpContent = new StringContent(putData);
            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            httpContent.Headers.ContentType.CharSet = "utf-8";
            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage response = httpClient.PutAsync(url, httpContent).Result;
                statusCode = response.StatusCode.ToString();
                if (response.IsSuccessStatusCode)
                {
                    result = response.Content.ReadAsStringAsync().Result;
                }
            }
            return result;
        }
    }
}
