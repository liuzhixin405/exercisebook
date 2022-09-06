using PubSubDemo.DataEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSubDemo.BusinessEntity
{
    internal abstract class PublishBase : IPublisher
    {
        protected IKeyedObjectStore<Article> articleStore; //订阅列表存储器
        protected IKeyedObjectStore<ArticleSubscription> subscriptionStore; //订阅情况存储器  订阅 取消订阅写入
        protected IKeyedObjectStore<Event> eventStore;   //订阅事件存储器  发布后写入

        public IEnumerator<Article> Articles
        {
            get
            {
                foreach (Article article in articleStore)
                {
                    yield return article;
                }
            }
        }

        public virtual void Publish(Article article)
        {
            if (article == null) throw new ArgumentNullException("article");
            Event e = new Event(article);
            eventStore.Save(e);
        }

        public virtual void Subscribe(Article article, ISubscriber subscriber)
        {
            if (article == null) throw new ArgumentNullException("article");
            if(subscriber == null) throw new ArgumentNullException("subscriber");
            ArticleSubscription subscription = new ArticleSubscription(article, subscriber);
            string key = ((IObjectWithKey)subscription).Key;
            if (subscriptionStore.Select(key) == null)
                subscriptionStore.Save(subscription);
        }

        public virtual void UnSubscribe(Article article, ISubscriber subscriber)
        {
            if(article == null) throw new ArgumentNullException("article");
            if (subscriber == null) throw new ArgumentNullException("subscriber");
            ArticleSubscription subscription = new ArticleSubscription(article, subscriber);
            subscriptionStore.Remove(((IObjectWithKey)subscription).Key);
        }
    }
}
