using Crawler.Models;
using Crawler.Selenium.Helper;
using Crawler.Service;
using Crawler.Service.Config;
using Crawler.Utility.HttpHelper;
using CrawlerConsole.TaskManager.Job;
using Microsoft.Extensions.DependencyInjection;
using RestSharp;
using System;
using System.Configuration;
using System.Net.Http;

namespace CrawlerConsole.DiService
{
    public static class ServiceDiExtension
    {
        public static void AddServer(this IServiceCollection services)
        {
            services.AddSingleton<BaseJob>();
            services.AddSingleton<RestClientHelper>();
            services.AddSingleton<RestClient>();
            services.AddSingleton<SeleniumHelper>();
        }
    }
}
