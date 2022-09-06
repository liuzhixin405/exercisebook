using PubSubDemo.DataEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSubDemo.Implement
{
    /// <summary>
    /// 预定事件持久化
    /// </summary>
    internal class ExtEventArgs:EventArgs
    {
        private Event e;
        public Event Event => e;
        public ExtEventArgs(Event e)
        {
            this.e = e;
        }
    }
}
