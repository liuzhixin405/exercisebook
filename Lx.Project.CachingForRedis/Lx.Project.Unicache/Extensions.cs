using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lx.Project.Unicache
{
    public static class Extensions
    {
        public static bool IsBlank(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static string ToJson<T>(this T t)
        {
           return JsonConvert.SerializeObject(t);
        }
        public static T FromJson<T>(this RedisValue val)
        {
            return JsonConvert.DeserializeObject<T>(val) ?? default;
        }
    }
}
