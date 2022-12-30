using ServiceStack.Redis;

namespace eapi.RedisHelper
{
    public class ConnectionHelper
    {
        public RedisClient Conn()
        {
            return new RedisClient("127.0.0.1", 6379);
        }
    }
}
