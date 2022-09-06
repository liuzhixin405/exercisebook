using PubSubDemo.DataEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSubDemo.Implement
{
    /// <summary>
    /// 预定情况持久化
    /// </summary>
    internal class ArticleSubscriptionStore:KeyedObjectStore<ArticleSubscription>
    {
    }
}
