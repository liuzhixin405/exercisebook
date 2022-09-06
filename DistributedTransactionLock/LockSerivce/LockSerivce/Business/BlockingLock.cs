using LockService.Helper;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace LockService.Business
{
    /// <summary>
    /// 阻塞锁
    /// </summary>
    public class BlockingLock
    {
        public static void Show(int i, string key, TimeSpan timeout)
        {
            using (var client = new ConnectionHelper().Conn())
            {
                using (var dataLock = client.AcquireLock("DataLock:" + key, timeout))
                {
                    //库存数量
                    var inventory = client.Get<int>("inventoryNum");
                    if (inventory > 0)
                    {
                        //库存-1
                        client.Set<int>("inventoryNum", inventory - 1);
                        //订单数量+1
                        var orderNum = client.Incr("orderNum");
                        inventory = client.Get<int>("inventoryNum");
                        Console.WriteLine($"{i}抢购成功***线程id:{Thread.CurrentThread.ManagedThreadId.ToString("00")},库存: {inventory},订单数量: {orderNum}");   
                    }
                    else
                    {
                        Console.WriteLine($"{i}抢购失败");
                    }
                    Thread.Sleep(100);
                }
            }
        }
    }
}