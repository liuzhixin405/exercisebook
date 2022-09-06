using Castle.DynamicProxy;
using StandardLibrary.HttpHelper;
using StandardLibrary.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StandardLibrary
{
    public class RequestApiProxy
    {
        public async static Task<string> InvokeApiAsync(string url,string parameters = null)
        {
            ProxyGenerator proxyGenerator = new ProxyGenerator();
            CustomInterceptor interceptor = new CustomInterceptor();
            RequestApi requestApi = proxyGenerator.CreateClassProxy<RequestApi>(interceptor);
            return await requestApi.InvokeApiAsync(url,parameters);
        }
    
    }
}
