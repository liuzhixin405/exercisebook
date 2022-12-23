
using ServiceStack.Redis;

namespace cat.RedisLocks
{
    public class CheckRepeatHelper
    {
        const string host = "127.0.0.1";
        const int port = 6379;
        public static bool CheckRepeat(string cachekey, string value, int secondsTimeout)
        {
            string NamespacePrefix = "api01_";
            string key = NamespacePrefix + cachekey;
            using (var client = new RedisClient(host, port))
            {
                var byts = System.Text.Encoding.UTF8.GetBytes(value);
                if (!string.IsNullOrEmpty(client.Get<string>(key)))
                {
                    return true;
                }
                client.Set(key, value, DateTime.Now.AddSeconds(secondsTimeout));//将Key缓存5秒
                return false;
            }
        }
    }
}
