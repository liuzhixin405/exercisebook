using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMNext
{
    internal class TestA
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Body { get; set; } = "aaaaaaaaa";
    }

    internal class TestB
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Body { get; set; }

    }

    internal static class Extension
    {
        internal static string ToJson(this object t)
        {
            return System.Text.Json.JsonSerializer.Serialize(t);  
        }
    }
}
