using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSubDemo.DataEntity
{
    /// <summary>
    /// publisher 抛出订阅事件
    /// </summary>
    internal class Event : IObjectWithKey
    {
        public string id=Guid.NewGuid().ToString();
        public string ID => id;
        private Article article;

        public Event(Article article)
        {
            this.article = article;
        }
        public virtual string Key => ID;

        internal Article Article { get => article;}
    }
}
