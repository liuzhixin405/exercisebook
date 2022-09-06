using EF.Core;
using System;

namespace Models
{
    public class User:ICacheKey      //该实例会被缓存起来
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public string GetCacheKey()
        {
            return "Users";
        }
    }
}
