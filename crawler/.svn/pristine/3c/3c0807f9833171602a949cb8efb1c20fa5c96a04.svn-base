using Crawler.Utility.HttpHelper;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace CrawlerConsole.token
{
    public class TokenHelper
    {
        private WebUtils _webUtils;
        public TokenHelper(WebUtils webUtils)
        {
            _webUtils = webUtils;
        }   
        /// <summary>
        /// 获取token
        /// </summary>
        /// <param name="url"></param>
        /// <param name="contentType"></param>
        /// <param name="jsonPars"></param>
        /// <returns></returns>
        public string GetToken(string url, string contentType, string jsonPars)
        {
            try
            {
                var result = _webUtils.DoPost(url, new Dictionary<string, object>(), contentType, jsonPars, false, null);
                return JObject.Parse(result).GetValue("data")["token"]?.ToString();
            }
            catch
            {
                throw;
            }
           
        }
    }
}
