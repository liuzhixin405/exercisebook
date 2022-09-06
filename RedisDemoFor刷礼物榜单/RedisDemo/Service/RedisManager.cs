using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedisDemo.Service
{
    public class RedisManager
    {
        private static RedisConfigInfo redisConfigInfo = new RedisConfigInfo();

        private static PooledRedisClientManager prcManager;

        static RedisManager()
        {
            CreateManager();
        }

        private static void CreateManager()
        {
            string[] WriteServerConStr = redisConfigInfo.WriteServerList.Split(',');
            string[] ReadServerConStr = redisConfigInfo.ReadServerList.Split(',');
            prcManager = new PooledRedisClientManager(ReadServerConStr, WriteServerConStr,new RedisClientManagerConfig
            {

                MaxWritePoolSize = redisConfigInfo.MaxWritePoolSize,
                MaxReadPoolSize = redisConfigInfo.MaxReadPoolSize,
                AutoStart = redisConfigInfo.AutoStart
            });
        }

        public static IRedisClient GetClient()
        {
            return prcManager.GetClient();
        }
    }
}
