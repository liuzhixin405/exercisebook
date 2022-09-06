using Microsoft.VisualStudio.TestTools.UnitTesting;
using PubSubPattern;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PubSubPatternTest
{
    /// <summary>
    /// 示例用的测试用内存持久化机制
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class KeyedObjectStore<T> : IKeyedObjectStore<T> where T : class, IObjectWithKey
    {
        protected IDictionary<string, T> data = new Dictionary<string, T>();

        public IEnumerator GetEnumerator()
        {
            foreach (T value in data.Values)
            {
                yield return value;
            }
        }

        public virtual void Remove(string key)
        {
            data.Remove(key);
        }

        public virtual void Save(T target)
        {
            if (target == null) throw new ArgumentNullException("target");
            data.Add(target.Key, target);
        }

        public virtual T Select(string key)
        {
            if (key == null) throw new ArgumentNullException("key");
            T result;
            if(data.TryGetValue(key,out result))
            {
                return result;
            }else
            {
                return null;
            }
        }
    }
    /// <summary>
    /// 示例用的预定事件持久化
    /// </summary>
    public class ExtEventArgs : EventArgs
    {
        private Event e;
        public Event Event
        {
            get { return e; }
        }

        public ExtEventArgs(Event e)
        {
            this.e = e;
        }
    }
    public class EventStore : KeyedObjectStore<Event>
    {
        public EventHandler<ExtEventArgs> NewEventOccured;
        public override void Save(Event target)
        {
            base.Save(target);
            if (NewEventOccured != null)
                NewEventOccured(this, new ExtEventArgs(target));
        }
    }

    public class NotificationEventArgs : EventArgs
    {
        private Notification notification;
        public Notification Notification => notification;
        public NotificationEventArgs(Notification notification)
        {
            this.notification = notification;
        }
    }
    public class NotificationStore : KeyedObjectStore<Notification>
    {
        public event EventHandler<NotificationEventArgs> NewNotificationOccured;
        public override void Save(Notification target)
        {
            base.Save(target);
            if (NewNotificationOccured != null)
                NewNotificationOccured(this, new NotificationEventArgs(target));
        }
    }
    /// <summary>
    /// 示例用的发布信息持久化
    /// </summary>
    public class ArticleStore : KeyedObjectStore<Article>
    {

    }
    /// <summary>
    /// 是利用的预定情况持久化
    /// </summary>
    public class ArticleSubscriptionStore : KeyedObjectStore<ArticleSubscription>
    {

    }
    /// <summary>
    /// 示例用的发布者
    /// </summary>
    public class Publisher : PublisherBase
    {
        public ArticleStore ArticleStore { set { articleStore = value; } }
        public ArticleSubscriptionStore ArticleSubscriptionStore { set { subscriptionStore = value; } }
        public EventStore EventStore { set { eventStore = value; } } 
    }
    /// <summary>
    /// 示例用的预定者
    /// </summary>
    public class Subscriber : SubscriberBase
    {
        public IDictionary<string,Queue<string>> Queue { get { return queue; } }
    }
    /// <summary>
    /// 示例用的通知生成器
    /// </summary>
    public class NotificationGenerator
    {
        private EventStore eventStore;
        private NotificationStore notificationStore;
        private ArticleSubscriptionStore articleSubscriptionStore;
        public NotificationGenerator(EventStore eventStore, NotificationStore notificationStore, ArticleSubscriptionStore articleSubscriptionStore)
        {
            this.eventStore = eventStore;
            this.notificationStore = notificationStore;
            this.articleSubscriptionStore = articleSubscriptionStore;

            eventStore.NewEventOccured += OnNewEventOccured;
        }
        /// <summary>
        /// 筛选并生成通知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNewEventOccured(object sender, ExtEventArgs args)
        {
            Event e = args.Event;
            string articlekey = e.Article.Key;
            foreach (ArticleSubscription item in articleSubscriptionStore)
            {
                string subscriptionArticleKey = (item as IObjectWithKey).Key;
                if (string.Equals(articlekey, subscriptionArticleKey))
                {
                    Notification notification = new Notification(e, item.Subscriber);
                    notificationStore.Save(notification);
                }
            }
        }
    }
    /// <summary>
    /// 示例用的通知发送对象
    /// </summary>
    public class Distributor
    {
        private NotificationStore notificationStore;
        public Distributor(NotificationStore notificationStore)
        {
            this.notificationStore = notificationStore;
            //发送新的通知情况
            notificationStore.NewNotificationOccured += OnNewNotificationOccured;
        }

        private void OnNewNotificationOccured(object sender, NotificationEventArgs e)
        {
            Article article = e.Notification.Event.Article;
            ISubscriber subscriber = e.Notification.Subscriber;
            subscriber.Enqueue(article);
        }
    }
    
    [TestClass]
    public class TestObjects
    {
        [TestMethod]
        public void Test()
        {
        }
    }
}
