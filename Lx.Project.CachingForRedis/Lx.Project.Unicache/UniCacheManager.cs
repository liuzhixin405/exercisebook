using System;
using StackExchange.Redis;
using System.Collections.Generic;
 
namespace Lx.Project.Unicache
{
    public static class UniCacheManager
    {
        private static IUniCache _cacheManager;

        static UniCacheManager()
        {
            _cacheManager = new UniRedisCache();
        }

        #region Methods


        public static bool Get<T>(string key, out bool hasValue, out T value) => _cacheManager.Get(key, out hasValue, out value);
        public static T Get<T>(string key) => _cacheManager.Get<T>(key);
        public static IDictionary<string, T> GetAll<T>(IEnumerable<string> keys) => GetAll<T>(keys);


        public static bool Del(string key) => _cacheManager.Del(key);
        public static bool DelAll(IEnumerable<string> keys) => _cacheManager.DelAll(keys);
        public static bool RemoveByPattern(string pattern) => _cacheManager.RemoveByPattern(pattern);


        public static bool SetAll<T>(IDictionary<string, T> values, int expiresSeconds) => _cacheManager.SetAll(values, expiresSeconds);
        public static bool Set<T>(string key, T value, int expiresSeconds) => _cacheManager.Set(key, value, expiresSeconds);
        public static bool SetWithDefaultExpiresSeconds<T>(string key, T value) => _cacheManager.SetWithDefaultExpiresSeconds(key, value);

        #endregion
    }
}
