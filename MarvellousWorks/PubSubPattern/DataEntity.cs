using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSubPattern
{
    /// <summary>
    /// 便于按照Key从持久层检索对象的抽象
    /// </summary>
    public interface IObjectWithKey
    {
        string Key { get; }
    }
    /// <summary>
    /// 预定发布内容对象
    /// 其内容可以是各种通用的数据类型,例如: string dataset stream xmldocument
    /// </summary>
    public class Article : IObjectWithKey
    {
        private string category;
        public string Category { get => category; set => category = value; }
        private string content;
        public string Content { get=>content; set=>content = value; }
        public Article(string category,string content)
        {
            this.category = category;
            this.content = content;
        }
        public virtual string Key => category;
    }
    /// <summary>
    /// Publisher 抛出的预定事件
    /// </summary>
    public class Event : IObjectWithKey
    {
        private string id = Guid.NewGuid().ToString();
        public Article Article { get => article; }
        private Article article;
        public virtual string Key => ID;
        public Event(Article artice)
        {
            this.article = artice;
        }
        public string ID { get => id; }
    }
    /// <summary>
    /// 订阅情况
    /// </summary>
    public class ArticleSubscription : IObjectWithKey
    {
        private Article article;
        public Article Article { get => article; }
        private ISubscriber subscriber;
        public ISubscriber Subscriber
        {
            get { return subscriber; }
        }
        public ArticleSubscription(Article article,ISubscriber subscriber)
        {
            this.article = article;
            this.subscriber = subscriber;
        }
        public virtual string Key
        {
            get
            {
                return ((IObjectWithKey)article).Key + subscriber.GetHashCode().ToString();
            }
        }
    }

    public class Notification : IObjectWithKey
    {
        private Event e;
        public Event Event { get { return e; } }

        private ISubscriber subscriber;
        public ISubscriber Subscriber
        {
            get { return subscriber; }
        }
        public Notification(Event e,ISubscriber subscriber)
        {
            this.e = e;
            this.subscriber = subscriber;
        }
        public string Key
        {
            get { return e.ID + subscriber.GetHashCode().ToString(); }
        }
    }
}
