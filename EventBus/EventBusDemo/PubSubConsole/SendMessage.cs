using EventBusCenter;
using EventBusCenter.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSubConsole
{
    public class SendMessage : EventData
    {
      public string Message { get; set; }
    }
}
