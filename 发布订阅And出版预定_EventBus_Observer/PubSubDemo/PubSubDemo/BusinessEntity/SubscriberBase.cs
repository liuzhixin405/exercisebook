using PubSubDemo.DataEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSubDemo.BusinessEntity
{
    internal abstract class SubscriberBase : ISubscriber
    {
        /// <summary>
        /// key 存 article的category  value存 article的 content 
        /// </summary>
        protected IDictionary<string, Queue<string>> queue = new Dictionary<string, Queue<string>>();

        public virtual void Enqueue(Article article)
        {
            if (article == null) throw new ArgumentNullException("article");
            string category = article.Category;
            if (!queue.ContainsKey(category))
            {
                queue.Add(category, new Queue<string>());
                queue[category].Enqueue(article.Content);
            }
        }
        public virtual string Dequeue(string category)
        {
            if (!queue.ContainsKey(category)) return null;
            if (queue[category].Count == 0) return null;
            return queue[category].Dequeue();
        }

        public virtual string Peek(string category)
        {
            if (!queue.ContainsKey(category)) return null;
            if(queue[category].Count==0)return null;
            return queue[category].Peek();
        }
    }
}
