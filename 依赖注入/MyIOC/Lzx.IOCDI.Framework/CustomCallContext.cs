using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Lzx.IOCDI.Framework
{
    public class CustomCallContext<T>
    {
        public static ConcurrentDictionary<string, AsyncLocal<T>> _callContextData = new ConcurrentDictionary<string, AsyncLocal<T>>();

        public static void SetData(string name,T data)
        {
            _callContextData.GetOrAdd(name, o => new AsyncLocal<T>()).Value = data;
        }

        public static T GetData(string name)
        {
            return _callContextData.TryGetValue(name, out AsyncLocal<T> data) ? data.Value : default(T);
        }
    }
}
