using PubSubDemo.DataEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSubDemo.Implement
{
    internal class EventStore:KeyedObjectStore<Event>
    {
        public EventHandler<ExtEventArgs> NewEventOccured;

        public override void Save(Event target)
        {
            base.Save(target);
            NewEventOccured?.Invoke(this,new ExtEventArgs(target));
        }
    }
}
