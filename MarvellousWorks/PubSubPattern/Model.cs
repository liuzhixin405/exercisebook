using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSubPattern
{
    /// <summary>
    /// 抽象Subscriber接口
    /// </summary>
    public interface ISubscriber
    {
        /// <summary>
        /// 接受预定
        /// </summary>
        /// <param name="article"></param>
        void Enqueue(Article article);
        /// <summary>
        /// 检查是否有某个分类的新预定结果
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        string Peek(string category);
        /// <summary>
        /// 获取某个分类新的预定结果
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>

        string Dequeue(string category);
    }

    /// <summary>
    /// 抽象Subscriber对象
    /// </summary>
    public abstract class SubscriberBase : ISubscriber
    {
        /// <summary>
        /// 按分类整理的接收队列
        /// 实际项目中这个队列可能就是一个内存缓冲,或持久化机制
        /// </summary>
        protected IDictionary<string, Queue<string>> queue = new Dictionary<string, Queue<string>>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public virtual string Dequeue(string category)
        {
            if (!queue.ContainsKey(category))
            {
                return null;
            }
            if(queue[category].Count == 0)
            {
                return null;
            }
            return queue[category].Dequeue();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="article"></param>
        public virtual void Enqueue(Article article)
        {
            if (article == null) throw new ArgumentNullException("article");
            string category = article.Category;
            if (!queue.ContainsKey(category))
            {
                queue.Add(category, new Queue<string>());
            }
            queue[category].Enqueue(article.Content);
        }

        public virtual string Peek(string category)
        {
            if (!queue.ContainsKey(category))
            {
                return null;
            }
            if (queue[category].Count == 0)
            {
                return null;
            }
            return queue[category].Peek();
        }
    }

    /// <summary>
    /// 抽象Publisher接口
    /// </summary>
    public interface IPubsher
    {
        /// <summary>
        /// 增加预定项
        /// </summary>
        /// <param name="article"></param>
        /// <param name="subscriber"></param>
        void Subscribe(Article article, ISubscriber subscriber);
        /// <summary>
        /// 撤销预定项
        /// </summary>
        /// <param name="article"></param>
        /// <param name="subscriber"></param>
        void UnSubscribe(Article article, ISubscriber subscriber);
        /// <summary>
        /// 获取所有可发布的内容列表
        /// </summary>
        IEnumerator<Article> Articles { get; }
        /// <summary>
        /// 发布预定事件
        /// </summary>
        /// <param name="article"></param>
        void Publish(Article article);
    }

    public abstract class PublisherBase : IPubsher
    {
        protected IKeyedObjectStore<Article> articleStore;
        protected IKeyedObjectStore<ArticleSubscription> subscriptionStore;
        protected IKeyedObjectStore<Event> eventStore;

        public virtual IEnumerator<Article> Articles
        {
            get
            {
                foreach (Article article in articleStore)
                {
                    yield return article;
                }
            }
        }

        public void Publish(Article article)
        {
            if (article == null) throw new ArgumentNullException("article");
            Event e = new Event(article);
            eventStore.Save(e);
        }

        public void Subscribe(Article article, ISubscriber subscriber)
        {
            if (article == null) throw new ArgumentNullException("article");
            if (subscriber == null) throw new ArgumentNullException("subscriber");
            ArticleSubscription subscription = new ArticleSubscription(article, subscriber);
            string key = ((IObjectWithKey)article).Key;
            if (subscriptionStore.Select(key) == null)
                subscriptionStore.Save(subscription);
            

        }

        public void UnSubscribe(Article article, ISubscriber subscriber)
        {
            if (article == null) throw new ArgumentNullException("article");
            if (subscriber == null) throw new ArgumentNullException("subscriber");
            ArticleSubscription subscription = new ArticleSubscription(article, subscriber);
            subscriptionStore.Remove(((IObjectWithKey)subscription).Key);
        }
    }
    /// <summary>
    /// 抽象的持久层对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IKeyedObjectStore<T> where T : class, IObjectWithKey
    {
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="target"></param>
        void Save(T target);
        /// <summary>
        /// 按照Key提取
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        T Select(string key);
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        void Remove(string key);
        /// <summary>
        /// 遍历
        /// </summary>
        /// <returns></returns>
        IEnumerator GetEnumerator();
    }
}
