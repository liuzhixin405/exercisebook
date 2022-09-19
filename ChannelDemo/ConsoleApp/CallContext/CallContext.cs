using System;
using System.Collections.Concurrent;
using System.Threading;

namespace CallContextTest
{
    public static class CallContext
    {
        static ConcurrentDictionary<string, AsyncLocal<object>> state = new ConcurrentDictionary<string, AsyncLocal<object>>();
        public static void SetData(string name, object data) => state.GetOrAdd(name, _ => new AsyncLocal<object>()).Value = data;

        public static object GetData(string name) => state.TryGetValue(name, out AsyncLocal<object> data) ? data.Value : null;
    }
    /// <summary>
    /// HttpContext.Current.Items 优选
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class CallContext<T>
    {
        static ConcurrentDictionary<string, AsyncLocal<T>> state = new ConcurrentDictionary<string, AsyncLocal<T>>();

        public static void SetData(string name, T data)
        {
            var result = state.GetOrAdd(name, _ => new AsyncLocal<T>());
            result.Value = data;
        }
        public static T GetData(string name)
        {
           state.TryGetValue(name, out AsyncLocal<T> data);
            return data.Value ?? default(T);
        }

    }
}
