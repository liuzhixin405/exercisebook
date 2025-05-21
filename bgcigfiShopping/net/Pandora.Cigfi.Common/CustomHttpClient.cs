using System.Net;
using System.Net.Http;

namespace Pandora.Cigfi.Common
{
    /// <summary>
    /// 忽略https证书的校验
    /// </summary>
    public class CustomHttpMessageHandler : HttpClientHandler
    {
        public CustomHttpMessageHandler()
        {
            this.AllowAutoRedirect = true;
            this.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            this.ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true;
        }
    }
    /// <summary>
    /// 忽略https证书的校验
    /// </summary>
    public class CustomHttpClientFactory : HttpClient
    {
        private HttpClient _client;
        public CustomHttpClientFactory(HttpClient client)
        {
            _client = client;
        }
        public HttpClient CreateClient()
        {
            return _client;
        }
    }
}
