using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
namespace Crawler.Utility.HttpHelper
{
    public class RestClientHelper
    {
        public async Task<string> PostRequestAsync(RestClient client,string host,string url,IDictionary<string,string> headers,IDictionary<string,string> pars,string jsonpars="")
        {
            //var client =new  RestSharp.RestClient(host);
            client.BaseUrl =new Uri( host);
            var request = new RestRequest(url, Method.POST);
            foreach (var item in headers)
            {
                request.AddHeader(item.Key, item.Value);
            }
            foreach (var item in pars)
            {
                request.AddJsonBody(JsonConvert.SerializeObject(pars));
            }
            if (!string.IsNullOrWhiteSpace(jsonpars))
            {
                request.AddJsonBody(jsonpars);
            }
            var content = await client.ExecuteAsync(request);
            return content.Content;
        }

        public async Task<string> GetRequestAsync(RestClient client, string host, string url, Dictionary<string, string> headers, Dictionary<string, string> pars=null)
        {
            //var client = new RestSharp.RestClient(host);
            client.BaseUrl = new Uri(host);
            var request = new RestRequest(url, Method.GET);
            foreach (var item in headers)
            {
                request.AddHeader(item.Key, item.Value);
            }
            if(pars!=null && pars.Count > 0)
            {
                foreach (var item in pars)
                {
                    request.AddJsonBody(JsonConvert.SerializeObject(pars));
                }
            }
          
            var content = await client.ExecuteAsync(request);
            return content.Content;
        }
    }
}
