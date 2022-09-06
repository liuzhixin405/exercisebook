using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMapperDemo
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
}
