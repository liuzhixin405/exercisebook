using RedisDemo.Service;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RedisDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            RankManager.Show();
            Console.Read();
        }
    }

    public class RankManager
    {
        private static List<string> userList = new List<string>
        {
            "User001","User002","User003","User004","User005","User006"
        };

        public static void Show()
        {
            RedisZSetService service = new RedisZSetService();
            
                service.FlushAll();
               
                RedisZSetService serviceNew = service;
               
                Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        foreach (var user in userList)
                        {
                            Thread.Sleep(1000);

                            serviceNew.IncrementItemInSortedSet("test", user, new Random().Next(1, 100));
                        }
                        Thread.Sleep(20 * 1000);
                    }

                });

                Task.Run(()=>{
                    while (true)
                    {
                        Thread.Sleep(1 * 1000);
                        Console.WriteLine("*******************当前排行*****************");
                        int i = 1;

                        foreach (var item in serviceNew.GetAllWithScoresFromSortedSet("test"))
                        {
                            Console.WriteLine($"第{i++}名 {item.Key} 礼物 {item.Value}");
                        }
                    }
                });
            
        }
    }
}
