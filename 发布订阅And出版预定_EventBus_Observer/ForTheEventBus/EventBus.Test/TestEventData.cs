using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Test
{
    internal class TestEventData:EventData
    {
        public int TestValue { get; set; }
        public TestEventData(int data)
        {
            TestValue = data;
        }
    }
}
