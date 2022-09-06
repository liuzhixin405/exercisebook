using PubSubDemo.BusinessEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSubDemo.Implement
{
    internal class Subscriber:SubscriberBase
    {
        public IDictionary<string, Queue<string>> Queue => queue;
    }
}
