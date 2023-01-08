using EventBusCenter.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSubConsole
{
    public class MessageEventHandler : IEventHandler<SendMessage>
    {
        public Task HandleEvent(SendMessage eventData)
        {
            Console.WriteLine($"接受消息为:{eventData.Message},时间：{eventData.EventTime}");
            return Task.CompletedTask;
        }
    }
}
