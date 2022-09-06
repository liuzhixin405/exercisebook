using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GettingStarted.RequestTest
{
    internal class CheckOrderStatusConsumer : IConsumer<CheckOrderStatus>
    {
        //respository get order
        public async Task Consume(ConsumeContext<CheckOrderStatus> context)
        {
            //Todo
            Console.WriteLine(context.Message.OrderId);
            await context.RespondAsync<OrderStatusResult>(new
            {
            OrderId = context.Message.OrderId,
            TimeSpan = DateTime.Now.TimeOfDay,
            StatusCode = 1,
            StatusText = "text"
            });
        }
    }
}
