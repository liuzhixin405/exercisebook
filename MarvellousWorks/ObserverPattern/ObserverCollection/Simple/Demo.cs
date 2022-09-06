using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObserverPattern.ObserverCollection.Simple
{
    /// <summary>
    /// 用于保存集合操作中操作条目信息的时间参数
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class DictionaryEventArgs<TKey, TValue> : EventArgs
    {
        private TKey key;
        private TValue value;

        public DictionaryEventArgs(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
        }

        public TKey Key => key;
        public TValue Value => value;
    }
    /// <summary>
    /// 具有操作事件的IDictionary<TKey,TValue>接口
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TVlaue"></typeparam>
    public interface IObserverableDictionary<TKey, TVlaue> : IDictionary<TKey, TVlaue>
    {
        EventHandler<DictionaryEventArgs<TKey,TVlaue>> NewItemAdded { get; set; }
    }

    /// <summary>
    /// 简单实现方式
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class ObserverableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IObserverableDictionary<TKey, TValue>
    {
        protected EventHandler<DictionaryEventArgs<TKey, TValue>> newItemAdded;
        public EventHandler<DictionaryEventArgs<TKey, TValue>> NewItemAdded 
        {   get => newItemAdded;
            set => newItemAdded = value; 
        }
        /// <summary>
        /// 为操作增加事件
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public new void Add(TKey key,TValue value)
        {
            base.Add(key, value);
            if (NewItemAdded != null)
                NewItemAdded(this, new DictionaryEventArgs<TKey, TValue>(key, value));
        }
    }

    
}

