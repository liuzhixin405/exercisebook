using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace LockService.Helper
{
    public class ConnectionHelper
    {
        public RedisClient Conn()
        {

            return new RedisClient("127.0.0.1", 6379);
        }
    }
}
