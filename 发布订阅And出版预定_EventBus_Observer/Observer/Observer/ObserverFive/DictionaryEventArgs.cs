using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObserverFive
{
    internal class DictionaryEventArgs<TKey,TValue> : EventArgs
    {
        private TKey key;
        private TValue value;
        public DictionaryEventArgs(TKey key,TValue value)
        {
            this.key = key;
            this.value = value;
        }

        public TKey Key => key;
        public TValue Value => value;
    }
}
