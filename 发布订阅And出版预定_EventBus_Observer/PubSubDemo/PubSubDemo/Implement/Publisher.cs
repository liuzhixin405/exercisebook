using PubSubDemo.BusinessEntity;
using PubSubDemo.DataEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSubDemo.Implement
{
    /// <summary>
    /// 出版者
    /// </summary>
    internal class Publisher:PublishBase
    {
        public ArticleStore ArticleStore { set { articleStore = value; } }
        public ArticleSubscriptionStore ArticleSubscriptionStore { set { subscriptionStore = value; } }
        public EventStore EventStore { set { eventStore = value; } }
    }
}
