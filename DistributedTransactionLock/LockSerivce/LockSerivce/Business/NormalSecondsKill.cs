using LockService.Helper;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Threading;

namespace LockService.Business
{
    /// <summary>
    /// lock锁
    /// </summary>
    public class NormalSecondsKill
    {
        private static readonly object _lock = new object();

        public static void Show()
        {

            lock (_lock)
            {
                using (var client = new ConnectionHelper().Conn())
                {
                    var inventory = client.Get<int>("inventoryNum");   //获取库存
                    if (inventory > 0)
                    {
                        client.Set<int>("inventoryNum", inventory - 1);    //库存-1
                        var orderNum = client.Incr("orderNum");             //订单+1
                        inventory = client.Get<int>("inventoryNum");
                        Console.WriteLine($"抢购成功*****线程id：{ Thread.CurrentThread.ManagedThreadId.ToString("00")},库存：{inventory},订单数量：{orderNum}");

                    }
                    else
                    {
                        Console.WriteLine("抢购失败");
                    }
                }
            }
        }
    }
}