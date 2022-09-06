using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;

namespace ConsoleAspNetCore
{
    class App
    {
        static async Task<int> Main(String[] args)
        {
            var builder = new HostBuilder()
                .ConfigureServices((context, service) =>
                {
                    service.AddHttpClient();
                    service.AddTransient<IPageBusiness, PageBusiness>();
                }).UseConsoleLifetime();
            var host = builder.Build();
            try
            {
                var pageBus = host.Services.GetRequiredService<IPageBusiness>();
                var pageContent = await pageBus.GetPage();
                await Console.Out.WriteLineAsync(pageContent.Substring(0, 500));
            }
            catch (Exception ex)
            {
                var logger = host.Services.GetRequiredService<ILogger<App>>();
                logger.LogError(ex, "An error occurred!");
            }


            return 0;
        }
    }

    internal interface IPageBusiness
    {
        Task<string> GetPage();
    }
    internal class PageBusiness : IPageBusiness
    {
        private readonly IHttpClientFactory _clientFactory;
        public PageBusiness(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }
        public async Task<string> GetPage()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://www.baidu.com");
            var client = _clientFactory.CreateClient();
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            return $"StatusCode:{response.StatusCode}";
        }
    }
}
