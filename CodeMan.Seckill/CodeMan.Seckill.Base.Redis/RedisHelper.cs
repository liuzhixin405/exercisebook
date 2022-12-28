using System;
using System.Collections.Concurrent;
using StackExchange.Redis;

namespace CodeMan.Seckill.Base.Redis
{
    public class RedisHelper
    {
        private readonly RedisOption _option;

        public RedisHelper(RedisOption option)
        {
            _option = option;
            _connections = new ConcurrentDictionary<string, ConnectionMultiplexer>();
        }

        private ConcurrentDictionary<string, ConnectionMultiplexer> _connections;

        private ConnectionMultiplexer GetConnection()
        {
            return _connections.GetOrAdd(_option.InstanceName,
                p => ConnectionMultiplexer.Connect(_option.Connection));
        }

        public IDatabase GetDatabase()
        {
            return GetConnection().GetDatabase(_option.DefaultDb);
        }

        /// <summary>
        /// 设置key的过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool Expire(string key, TimeSpan time)
        {
            try
            {
                GetDatabase().KeyExpire(key, time);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public TimeSpan GetExpire(string key)
        {
            try
            {
                // ReSharper disable once PossibleInvalidOperationException
                TimeSpan time = GetDatabase().KeyTimeToLive(key).Value;
                return time;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        /// <summary>
        /// 写入缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Set(string key, RedisValue value)
        {
            bool result = false;
            try
            {
                result = GetDatabase().StringSet(key, value);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return result;
        }

        /// <summary>
        /// 写入缓存并设置过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireTime"></param>
        /// <returns></returns>
        public bool Set(string key, string value, TimeSpan expireTime)
        {
            Console.WriteLine($"过期时间:{expireTime}");
            bool result = false;
            try
            {
                result = GetDatabase().StringSet(key, value);
                GetDatabase().KeyExpire(key, expireTime);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return result;
        }

        public RedisValue Get(string key)
        {
            return key == null ? "" : GetDatabase().StringGet(key);
        }

        /// <summary>
        /// 递增
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public long Incr(string key, long value)
        {
            return GetDatabase().StringIncrement(key, value);
        }

        public long Incr(string key)
        {
            return GetDatabase().StringIncrement(key);
        }

        /// <summary>
        /// 递减
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public long Decr(string key, long value)
        {
            return GetDatabase().StringIncrement(key, value);
        }

        public long Decr(string key)
        {
            return GetDatabase().StringDecrement(key);
        }

        public RedisValue Hget(string key, string item)
        {
            return GetDatabase().HashGet(key, item);
        }

        /// <summary>
        /// 加锁，如果锁定成功，就去执行方法
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expire"></param>
        /// <returns></returns>
        public bool TryGetLock(string key, string value, TimeSpan expire)
        {
            return GetDatabase().LockTake(key, value, expire);
        }

        /// <summary>
        /// 解锁
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool LockRelease(string key, string value)
        {
            return GetDatabase().LockRelease(key, value);
        }

        public bool Remove(string key)
        {
            return GetDatabase().KeyDelete(key);
        }

    }
}