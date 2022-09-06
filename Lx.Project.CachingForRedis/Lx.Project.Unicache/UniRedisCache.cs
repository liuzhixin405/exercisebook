using System;
using StackExchange.Redis;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Lx.Project.Unicache
{
    public class UniRedisCache:IUniCache
    {
        public ConnectionMultiplexer RedisConnection{get;private set;}
        public int DefaultExpiresSeconds{get;}
        private bool _exitFlag;

        public UniRedisCache(string host="",int defaultExpiresSeconds=1200)
        {
            DefaultExpiresSeconds = defaultExpiresSeconds;
            var mRedisHost = string.IsNullOrWhiteSpace(host)?"8.142.71.127:6379": host;
            var config = ConfigurationOptions.Parse(mRedisHost);
              config.KeepAlive = 180;
            //config.User = "";
            //config.Password = "";
            config.ConnectRetry = 3;
            config.ConnectTimeout = 2000;

            RedisConnection = ConnectionMultiplexer.Connect(config);
        }

        #region Utilities

        public IDatabase GetDatabase()
        {
            if (_exitFlag) return null;
            return RedisConnection.GetDatabase();
        }

        public int GetExpiredTime(int expiresSeconds = 0)
        {
            if (expiresSeconds > 0) return expiresSeconds + new Random().Next(2);
            return DefaultExpiresSeconds + new Random().Next(2);
        }

        #endregion

        #region Methods

        /// <summary>
        /// 退出 redis
        /// </summary>
        public void Exit()
        {
            _exitFlag = true;
            RedisConnection.Dispose();
            RedisConnection = null;
        }

        public bool Del(string key)
        {
            if (_exitFlag) return false;
            if (key.IsBlank()) return false;
            bool deleteRes;

            var redis = GetDatabase();
            try
            {
                deleteRes = redis.KeyDelete(key);
            }
            catch
            {
                return false;
            }
            return deleteRes;
        }

        public bool RemoveByPattern(string pattern)
        {
            if (_exitFlag) return false;
            if (pattern.IsBlank()) return false;
            var deleteNum = 0L;
            try
            {
                var redis = GetDatabase();
                foreach (var ep in RedisConnection.GetEndPoints())
                {
                    var server = RedisConnection.GetServer(ep);
                    var keys = server.Keys(pattern: "*" + pattern + "*", database: redis.Database);
                    deleteNum += redis.KeyDelete(keys.ToArray());
                }
            }
            catch
            {
                return false;
            }
            return deleteNum > 0;
        }
        public bool DelAll(IEnumerable<string> keys)
        {
            if (_exitFlag) return false;
            if (keys == null) return false;

            var deleteRes = true;
            if (!keys.Any()) return true;
            try
            {
                var redis = GetDatabase();
                foreach (var key in keys)
                {
                    deleteRes = deleteRes && redis.KeyDelete(key);
                }
            }
            catch
            {
                return false;
            }
            return deleteRes;
        }

        public bool SetAll<T>(IDictionary<string, T> values, int expiresSeconds)
        {
            if (_exitFlag) return false;
            if (!values.Any()) return true;

            var expiryTime = TimeSpan.FromSeconds(GetExpiredTime(expiresSeconds));
            var redis = GetDatabase();
            try
            {
                foreach (var kv in values)
                {
                    redis.StringSet(kv.Key, kv.Value.ToJson(), expiryTime);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool Set<T>(string key, T value, int expiresSeconds)
        {
            if (_exitFlag) return false;
            if (key.IsBlank()) return true;

            var expiryTime = TimeSpan.FromSeconds(GetExpiredTime(expiresSeconds));
            var redis = GetDatabase();
            try
            {
                redis.StringSet(key, value.ToJson(), expiryTime);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool SetWithDefaultExpiresSeconds<T>(string key, T value)
        {
            return Set<T>(key, value, 0);
        }


        public bool Get<T>(string key, out bool hasValue, out T value)
        {
            value = default;
            hasValue = false;
            if (_exitFlag) return false;
            if (key.IsBlank()) return false;

            var redis = GetDatabase();

            try
            {
                var json = redis.StringGet(key);
                value =json.FromJson<T>();
                hasValue = !EqualityComparer<T>.Default.Equals(value, default(T));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public T Get<T>(string key)
        {
            if (!Get(key, out _, out T value)) return default;
            return value;
        }

        public IDictionary<string, T> GetAll<T>(IEnumerable<string> keys)
        {
            if (_exitFlag) return null;

            try
            {
                IDictionary<string, T> bags = new Dictionary<string, T>();
                foreach (var key in keys)
                {
                    bags.Add(key, Get<T>(key));
                }
                return bags;
            }
            catch
            {
                return null;
            }
        }

        #endregion

    }
}
