using Microsoft.VisualStudio.TestTools.UnitTesting;
using PubSubPatternTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSubPattern.Test
{
    [TestClass]
    public class TestPubSub
    {
        #region Private Fields
        private ArticleStore articleStore;
        private Article articleX;
        private Article articleY;
        private Article articleZ;
        private ArticleSubscriptionStore articleSubscriptionStore;
        private EventStore eventStore;
        private NotificationStore notificationStore;
        private Publisher publisher;
        private Subscriber subscriberA;
        private Subscriber subscriberB;
        private NotificationGenerator generator;
        private Distributor distributor;
        #endregion

        #region PreProcess

        #region 初始化久层，增加增加新的预定事件
        void InitPersistence()
        {
            articleStore = new ArticleStore();
            articleX = new Article("X", string.Empty);
            articleY = new Article("Y", string.Empty);
            articleZ = new Article("Z", string.Empty);
            articleStore.Save(articleX);
            articleStore.Save(articleY);
            articleStore.Save(articleZ);
            articleSubscriptionStore = new ArticleSubscriptionStore();
            eventStore = new EventStore();
            notificationStore = new NotificationStore();
        }
        #endregion

        #region 组装出版——预定机制
        void AssemblyPubSub()
        {
            // 构造发布者
            publisher = new Publisher();
            publisher.ArticleStore = articleStore;
            publisher.ArticleSubscriptionStore = articleSubscriptionStore;
            publisher.EventStore = eventStore;

            // 构造预定者
            subscriberA = new Subscriber();
            subscriberB = new Subscriber();

            // 通知生成和分发
            generator = new NotificationGenerator(eventStore,
                notificationStore, articleSubscriptionStore);
            distributor = new Distributor(notificationStore);

            // 预定新的事件
            publisher.Subscribe(articleX, subscriberA);
            publisher.Subscribe(articleY, subscriberA);
            publisher.Subscribe(articleY, subscriberB);
            publisher.Subscribe(articleZ, subscriberB);
        }
        #endregion

        #region 执行发布
        void PublishEvent()
        {
            publisher.Publish(new Article("X", "1"));
            publisher.Publish(new Article("X", "2"));
            publisher.Publish(new Article("Y", "3"));
            publisher.Publish(new Article("Z", "4"));
        }
        #endregion

        #endregion

        [TestMethod]
        public void Test()
        {
            InitPersistence();
            AssemblyPubSub();
            PublishEvent();

            // 验证出版——预定体系执行效果
            Assert.AreEqual<int>(2, subscriberA.Queue.Count);
            Assert.AreEqual<string>(subscriberA.Dequeue("X"), "1");
            Assert.AreEqual<string>(subscriberA.Dequeue("X"), "2");
            Assert.AreEqual<string>(subscriberA.Dequeue("Y"), "3");

            Assert.AreEqual<int>(2, subscriberB.Queue.Count);
            Assert.AreEqual<string>(subscriberB.Dequeue("Y"), "3");
            Assert.AreEqual<string>(subscriberB.Dequeue("Z"), "4");
        }
    }
}
