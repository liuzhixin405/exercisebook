using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Crawler.Models
{
    /// <summary>
    /// 带系统参数数据   data object避免单条和多条返回转换失败报错
    /// </summary>
    public class ResponseMessage
    {
        public string code { get; set; }
        public object data { get; set; }
        public string message { get; set; }
        public string meta { get; set; }
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        public string totalCount { get; set; }
    }

    /// <summary>
    /// 队列数据
    /// </summary>
    public class JData
    {
        public int level { get; set; }
        public string targetUrl { get; set; }
        public string action { get; set; }
        public string cookie { get; set; }
        public object parameters { get; set; }
        public string postBackUrl { get; set; }
        public string timeStamp { get; set; }
        public DateTime createOnUtc { get; set; }
        public string id { get; set; }
    }
}
