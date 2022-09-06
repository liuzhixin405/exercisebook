using PubSubDemo.DataEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSubDemo.BusinessEntity
{
    /// <summary>
    /// 抽象发接口
    /// </summary>
    internal interface IPublisher
    {
        void Subscribe(Article article,ISubscriber subscriber);
        void UnSubscribe(Article article,ISubscriber subscriber);
        IEnumerator<Article> Articles { get; }
        void Publish(Article article);
    }
}
