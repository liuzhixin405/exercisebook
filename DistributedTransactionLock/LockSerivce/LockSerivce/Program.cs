using LockService.Helper;
using LockService.Business;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LockService
{
    class Program
    {
        //需开启多个控制台一起跑测试
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder().AddCommandLine(args);
            var configuration = builder.Build();
            
            int minute = int.Parse(configuration["minute"]);   //设置开始秒杀时间
            int type = int.Parse(configuration["type"]);       //设置锁类型         1 lock锁 2  阻塞锁 3 非阻塞锁                            
            using (var client = new ConnectionHelper().Conn())
            {
                //设置库存10
                client.Set<int>("inventoryNum", 10);
                //订单  设置订单10
                client.Set<int>("orderNum", 10);
                Console.WriteLine($"在{minute}分0秒正式开始秒杀! ");
                var flag = true;
                while (flag)
                {
                    if (DateTime.Now.Minute == minute)
                    {
                        flag = false;

                        Parallel.For(0, 30, (i) => {

                            int temp = i;
                            //三种锁如下:
                            switch (type)
                            {
                                case 1:
                                    NormalSecondsKill.Show(); //lock锁  会出现超卖情况 。 原因是非同一个线程锁不住
                                    break;
                                case 2:
                                    BlockingLock.Show(i, "akey", TimeSpan.FromSeconds(100));   //阻塞锁 ，可防止超卖，速度比不上非阻塞锁
                                    break;
                                case 3:
                                    ImmediatelyLock.Show(i, "akey", TimeSpan.FromSeconds(100));   //非阻塞锁 ，会出现卖不完情况
                                    break;
                                default:
                                    throw new Exception("类型不正确");
                            }
                        });
                        Thread.Sleep(100);
                    }
                }
            }
            Console.ReadKey();
        }
    }
}
