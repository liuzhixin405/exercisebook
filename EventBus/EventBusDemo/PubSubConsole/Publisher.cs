using EventBusCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PubSubConsole
{
    public class Publisher
    {
        public Task Notification(string message)
        {
            SendMessage sendMessage = new SendMessage() { Message = message };
            sendMessage.EventStore = this;
            EventBus.Default.Trigger(sendMessage);
            return Task.CompletedTask;
        }
    }
}
