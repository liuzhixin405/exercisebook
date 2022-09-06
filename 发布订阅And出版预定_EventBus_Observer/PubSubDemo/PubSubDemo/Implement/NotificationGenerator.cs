using PubSubDemo.DataEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSubDemo.Implement
{
    /// <summary>
    /// 通知生成器
    /// </summary>
    internal class NotificationGenerator
    {
        private EventStore eventStore;
        private NotificationStore notificationStore;
        private ArticleSubscriptionStore articleSubscriptionStore;
        public NotificationGenerator(EventStore eventStore,NotificationStore notificationStore,ArticleSubscriptionStore articleSubscriptionStore)
        {
            this.eventStore = eventStore;
            this.notificationStore = notificationStore;
            this.articleSubscriptionStore = articleSubscriptionStore;
            //接受预定通知
            eventStore.NewEventOccured += OnNewEventOccured;
        }

        /// <summary>
        /// 筛选并生成通知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnNewEventOccured(object sender, ExtEventArgs args)
        {
            Event e = args.Event;
            string articleKey = e.Article.Key;
            foreach (ArticleSubscription articleSubscription in articleSubscriptionStore)
            {
                string subscriptionArticleKey = ((IObjectWithKey)articleSubscription.Article).Key;
                if(string.Equals(subscriptionArticleKey, articleKey))
                {
                    Notification notification = new Notification(e, articleSubscription.Subscriber);
                    notificationStore.Save(notification);
                }
            }
        }
    }
}
