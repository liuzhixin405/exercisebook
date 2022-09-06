using System;
using RestSharp;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace ConsoleWebRequest
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var guid = Guid.NewGuid().ToString();
            var sign = ToMD5String("123456789" + time + guid +""+ "asdfdghjgfd");
            var client = new RestSharp.RestClient("http://localhost:5000/WeatherForecast");   //appId+time+guid+body+appSecret
            
            var request = new RestRequest().AddHeader("appid", "123456789").AddHeader("time", time).AddHeader("guid", guid).AddHeader("sign", sign);
            var result = await client.GetAsync(request);
            Console.WriteLine(result.Content);
        }

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
