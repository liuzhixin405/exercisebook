using System;
using RestSharp;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;

namespace ConsoleWebRequest
{
    internal class Program
    {

        static async Task Main(string[] args)
        {
            var guid = Guid.NewGuid().ToString();
            var time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            var str =new string[]{ "id"};
            ;
            var sign = ("ctAdmin" + ToJsTimestamp(Convert.ToDateTime(time)) + guid + System.Text.Json.JsonSerializer.Serialize(str) + "IVh9LLSVFcoQPQ5K");
            var client = new RestSharp.RestClient("http://10.10.1.28:5000/api/OpenContract/getassets");   //appId+time+guid+body+appSecret
            var request = new RestRequest()
                .AddHeader("appid", "ctAdmin")
                .AddHeader("time", time)
                .AddHeader("guid", guid)
                .AddHeader("sign", ToMD5String(sign))
                .AddHeader("Content-Type", "application/json")
                ;

            //request.AddParameter("PageIndex", "1", ParameterType.QueryString)
            //    .AddParameter("PageRows", "20", ParameterType.QueryString)
            //    .AddParameter("SortField", "LastUpdateTime", ParameterType.QueryString)
            //    .AddParameter("SortType", "desc", ParameterType.QueryString);
            request.Method = Method.POST;
            //request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", System.Text.Json.JsonSerializer.Serialize(str), ParameterType.RequestBody);
            var result = await client.ExecuteAsync(request);
            Console.WriteLine(result.Content);
        }
        //        static async Task Main(string[] args)
        //        {
        //            var time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //            var body = new List<string> { "1447508705602965504" };
        //            var str = new { CustomerIds = new string[] { "1459069634991886336" ,null}, CoinId="", BillType=0, BillTradeType=0 };
        //;            var guid = Guid.NewGuid().ToString();
        //            var sign = ("AppAdmin" + ToJsTimestamp(Convert.ToDateTime(time)) + guid + System.Text.Json.JsonSerializer.Serialize(str) + "IVh9LLSVFcoQPQ5K");
        //            var client = new RestSharp.RestClient("http://localhost:5000/api/OpenContract/GetCustomerBillingRecord");   //appId+time+guid+body+appSecret
        //            var request = new RestRequest()
        //                .AddHeader("appid", "AppAdmin")
        //                .AddHeader("time", time)
        //                .AddHeader("guid", guid)
        //                .AddHeader("sign", ToMD5String(sign))
        //                .AddHeader("Content-Type", "application/json")
        //                ;

        //            //request.AddParameter("PageIndex", "1", ParameterType.QueryString)
        //            //    .AddParameter("PageRows", "20", ParameterType.QueryString)
        //            //    .AddParameter("SortField", "LastUpdateTime", ParameterType.QueryString)
        //            //    .AddParameter("SortType", "desc", ParameterType.QueryString);
        //            request.Method = Method.POST;
        //            //request.AddHeader("Content-Type", "application/json");
        //            request.AddParameter("application/json", System.Text.Json.JsonSerializer.Serialize(str), ParameterType.RequestBody);
        //            var result = await client.ExecuteAsync(request);
        //            Console.WriteLine(result.Content);
        //        }

        //static async Task Main(string[] args)
        //{
        //    var time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //    var body = new List<string> { "1501378623351820288" };
        //    var str = new { PageIndex=1,PageRpows=20,SortField="CreateTime",SortType="Desc",Search= new {CustomerId = new string[] { "1534453482038235136", "1534480602567282688", "1534469931024781312", "1534479459611054080", "1534479855985364992" } }, StartTime =DateTime.Parse("2022-06-01 00:00:00") , EndTime= DateTime.Parse("2022-06-09 00:00:00"), IsLiquidationOrder=false };
        //    ; var guid = Guid.NewGuid().ToString();
        //    var sign = ("AppAdmin" + ToJsTimestamp(Convert.ToDateTime(time)) + guid + System.Text.Json.JsonSerializer.Serialize(str) + "IVh9LLSVFcoQPQ5K");
        //    var client = new RestSharp.RestClient("https://prectadminapi.idcs.io/api/OpenContract/GetTradeList");   //appId+time+guid+body+appSecret
        //    var request = new RestRequest()
        //        .AddHeader("appid", "AppAdmin")
        //        .AddHeader("time", time)
        //        .AddHeader("guid", guid)
        //        .AddHeader("sign", ToMD5String(sign))
        //        .AddHeader("Content-Type", "application/json")
        //        ;

        //    //request.AddParameter("PageIndex", "1", ParameterType.QueryString)
        //    //    .AddParameter("PageRows", "20", ParameterType.QueryString)
        //    //    .AddParameter("SortField", "LastUpdateTime", ParameterType.QueryString)
        //    //    .AddParameter("SortType", "desc", ParameterType.QueryString);
        //    request.Method = Method.POST;
        //    //request.AddHeader("Content-Type", "application/json");
        //    request.AddParameter("application/json", System.Text.Json.JsonSerializer.Serialize(str), ParameterType.RequestBody);
        //    var result = await client.ExecuteAsync(request);
        //    Console.WriteLine(result.Content);
        //}

        //static async Task Main(string[] args)
        //{
            //var guid = Guid.NewGuid().ToString();
            //var time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
           
            //var str = new {  StartTime = DateTime.Parse("2022-07-07 00:00:00"), EndTime = DateTime.Parse("2022-07-08 00:00:00"), Minute = 3 };
            //; 
            //var sign = ("ctAdmin" + ToJsTimestamp(Convert.ToDateTime(time)) + guid + System.Text.Json.JsonSerializer.Serialize(str) + "IVh9LLSVFcoQPQ5K");
            //var client = new RestSharp.RestClient("https://ctadminapi.idcs.io/api/OpenContract/GetOpenCloseOrderByTime");   //appId+time+guid+body+appSecret
            //var request = new RestRequest()
            //    .AddHeader("appid", "ctAdmin")
            //    .AddHeader("time", time)
            //    .AddHeader("guid", guid)
            //    .AddHeader("sign", ToMD5String(sign))
            //    .AddHeader("Content-Type", "application/json")
            //    ;

            ////request.AddParameter("PageIndex", "1", ParameterType.QueryString)
            ////    .AddParameter("PageRows", "20", ParameterType.QueryString)
            ////    .AddParameter("SortField", "LastUpdateTime", ParameterType.QueryString)
            ////    .AddParameter("SortType", "desc", ParameterType.QueryString);
            //request.Method = Method.POST;
            ////request.AddHeader("Content-Type", "application/json");
            //request.AddParameter("application/json", System.Text.Json.JsonSerializer.Serialize(str), ParameterType.RequestBody);
            //var result = await client.ExecuteAsync(request);
            //Console.WriteLine(result.Content);
        //}
        static string ToMD5String(string str)
        {
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(str);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("x2"));
            }
            md5.Dispose();

            return sb.ToString();
        }
        /// <summary>
        /// 获取Js格式的timestamp
        /// </summary>
        /// <param name="dateTime">日期</param>
        /// <returns></returns>
        static long ToJsTimestamp(DateTime dateTime)
        {
            var startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();
            long result = (dateTime.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位
            return result;
        }
    }
}
