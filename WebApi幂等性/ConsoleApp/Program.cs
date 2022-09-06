using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp
{
    using NServiceKit.Redis;// 通过nuget添加redis库
    class Program
    {

        static void Main(string[] args)
        {
            var m = 10;
            while (m < 20)
            {
                m++;
                ///模拟重复发送3次请求
                for (var j = 10; j <= 20; j++)
                {
                    CreateOrderApi(j);
                }
                //for (var i = 1; i <= 3; i++)
                //{//模拟重复发送3次请求
                //    Thread t2 = new Thread(CreateOrderApi);
                //    t2.Start();
                //}
               
            }
            Console.ReadLine();
        }
        /// <summary>
        /// 比如说这是创建订单方法，
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static void CreateOrderApi(int request_id)
        {
            string parmaterid = "p2";//假设这是api请求参数id

            var nxkey = "cnx" + parmaterid;
            var value = parmaterid;
            bool setnx = SetNX(nxkey, value,5);
            if (!setnx)
            {
                Console.WriteLine("requestid: " + request_id.ToString() + " " + "请求太频繁，请10秒后再试。");
                return;
            }
            //todo: 这里写订单逻辑

            Console.WriteLine("requestid: " + request_id.ToString() + " " + "Success");
        }

        const string host = "127.0.0.1";
        const int port = 6379;
        public static bool SetNX(string cachekey, string value, int secondsTimeout)
        {
            string NamespacePrefix = "api01_";
            string key = NamespacePrefix + cachekey;
            using (var client = new RedisClient(host, port))
            {
                var byts = System.Text.Encoding.UTF8.GetBytes(value);
                var result = client.SetNX(key, byts);
                var setnx = (result == null) ? true : false;
                client.Set(key, value, DateTime.Now.AddSeconds(secondsTimeout));//将Key缓存5秒
                return setnx;
            }
        }

    }
}