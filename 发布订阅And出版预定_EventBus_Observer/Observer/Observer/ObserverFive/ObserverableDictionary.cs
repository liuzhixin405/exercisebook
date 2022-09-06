using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObserverFive
{
    internal class ObserverableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IObserverableDictionary<TKey, TValue>
    {
        protected EventHandler<DictionaryEventArgs<TKey, TValue>> newItemAdded;
        public EventHandler<DictionaryEventArgs<TKey, TValue>> NewItemAdded { get => newItemAdded;set=> newItemAdded = value;}
        public new void Add(TKey key,TValue value)
        {
            base.Add(key, value);
            if(NewItemAdded != null)
                NewItemAdded(this, new DictionaryEventArgs<TKey, TValue>(key, value));  
        }
    }
}
