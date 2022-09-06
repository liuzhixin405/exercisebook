using PubSubDemo.DataEntity;
using PubSubDemo.Implement;
using System;

namespace PubSubDemo
{

    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("*********测试开始*********");
            new App().DoTest();
            Console.WriteLine("*********测试结束*********");
        }
    }
    internal class App
    {
        #region private fields
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
        internal void DoTest()
        {
           
            InitPersistence();
            AssemblySub();
            PublishEvent();

            Console.WriteLine(subscriberA.Queue.Count);
            Console.WriteLine(subscriberA.Dequeue("X"));
            Console.WriteLine(subscriberA.Dequeue("X"));
            Console.WriteLine(subscriberA.Dequeue("Y"));

            Console.WriteLine(subscriberA.Queue.Count);

            Console.WriteLine(subscriberA.Dequeue("Y"));
            Console.WriteLine(subscriberA.Dequeue("Z"));


            Console.WriteLine("OVER!!!");
        }

        /// <summary>
        /// 初始化持久层，增加新的预定事件
        /// </summary>
        void InitPersistence()
        {
            articleStore = new ArticleStore();
            articleX = new Article("X", String.Empty);
            articleY = new Article("Y", String.Empty);
            articleZ = new Article("Z", String.Empty);

            articleStore.Save(articleX);
            articleStore.Save(articleY);
            articleStore.Save(articleZ);

            articleSubscriptionStore = new ArticleSubscriptionStore();
            eventStore = new EventStore();
            notificationStore = new NotificationStore();
        }

        /// <summary>
        /// 组装出版-预定机制
        /// </summary>
        void AssemblySub()
        {
            publisher = new Publisher();

            publisher.ArticleStore = articleStore;
            publisher.ArticleSubscriptionStore = articleSubscriptionStore;
            publisher.EventStore = eventStore;

            subscriberA = new Subscriber();
            subscriberB = new Subscriber();

            generator = new NotificationGenerator(eventStore, notificationStore, articleSubscriptionStore);
            distributor = new Distributor(notificationStore);

            publisher.Subscribe(articleX, subscriberA);
            publisher.Subscribe(articleY, subscriberA);
            publisher.Subscribe(articleY, subscriberB);
            publisher.Subscribe(articleZ, subscriberB);
        }

        /// <summary>
        /// 执行发布
        /// </summary>
        void PublishEvent()
        {
            publisher.Publish(new Article("X", "1"));
            publisher.Publish(new Article("X", "2"));
            publisher.Publish(new Article("Y", "3"));
            publisher.Publish(new Article("Z", "4"));
        }
    }
}
