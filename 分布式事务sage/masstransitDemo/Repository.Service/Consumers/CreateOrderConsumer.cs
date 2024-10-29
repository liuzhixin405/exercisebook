using MassTransit;
using MassTransit.Transports;
using Repository.Service.Dtos;
using Repository.Service.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Repository.Service.Consumers
{
    public class CreateOrderConsumer : IConsumer<ICreateTransaction>
    {
        private readonly OrderService _orderService;

        public CreateOrderConsumer(OrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task Consume(ConsumeContext<ICreateTransaction> context)
        {
            ICreateTransaction dto = null;
            try
            {
                dto = context.Message;
                var order = new Order { CustomerName = dto.CustomerName, OrderDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds(), TotalAmount = dto.TotalAmount };
                var result = await _orderService.CreateOrderAsync(order);
                if (result <= 0)
                {
                    await context.Publish<ITransactionCreated>(new TransactionCreated
                    {
                        Message = $"创建订单失败，订单号：{JsonSerializer.Serialize(order)}",
                        Success = false
                    });
                }
                else
                {
                    await context.Publish<ITransactionCreated>(new TransactionCreated
                    {
                        Message = $"创建订单成功，订单号：{order.Id}",
                        Success = false
                    });
                }
            }
            catch (Exception ex)
            {
                await context.Publish<ITransactionCreated>(new TransactionCreated
                {
                    Message = $"创建订单失败，订单号：{JsonSerializer.Serialize(dto)}",
                    Success = false
                });
            }
            finally
            {

            }
        }
    }
}
