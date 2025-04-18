﻿using Crawler.Common;
using Crawler.Models;
using Crawler.Selenium.Helper;
using Crawler.Service.Config;
using Crawler.Utility.HttpHelper;
using CrawlerConsole.DiService;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace CrawlerConsole.TaskManager.Job
{
    /// <summary>
    /// 公共模块
    /// </summary>
    public class BaseJob
    {

        public RestClientHelper restClientHelper;
        public RestClient restClient;
        public SeleniumHelper seleniumHelper;
        public string tokenString;
        public BaseJob(SeleniumHelper seleniumHelper,RestClientHelper restClientHelper,RestClient restClient)
        {
            this.restClientHelper = restClientHelper;
            this.restClient = restClient;
            this.seleniumHelper = seleniumHelper;

            if (string.IsNullOrWhiteSpace(tokenString))
            {
                try
                {
                    var result =restClientHelper.PostRequestAsync(restClient, ApplicationConfig.Configuration["ReqUrl:DBHost"]?.ToString(),"/token", new Dictionary<string, string>(), new Dictionary<string, string>(), ApplicationConfig.Configuration["DataBaseUserInfo"]).Result;
                    tokenString = JObject.Parse(result).GetValue("data")["token"]?.ToString();
                    CommonHelper.ConsoleAndLogger($"token = {tokenString}", CommonHelper.LoggerType.Info);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"token获取失败: {ex.Message}");
                }
            }
        }
    }
}
