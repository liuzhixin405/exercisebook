using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObserverFive
{
    internal interface IObserverableDictionary<TKey,TValue>:IDictionary<TKey, TValue>
    {
        EventHandler<DictionaryEventArgs<TKey,TValue>> NewItemAdded { get; set; }
    }
}
