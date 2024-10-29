using MassTransit;
using Repository.Service.Dtos;
using Repository.Service.Orders;
using Repository.Service.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Repository.Service.Consumers
{
    public class CreateProductConsumer : IConsumer<ICreateTransaction>
    {
        private readonly ProductService _productService;
        public CreateProductConsumer(ProductService productService)
        {
            _productService = productService;
        }
        public async Task Consume(ConsumeContext<ICreateTransaction> context)
        {
            ICreateTransaction dto = null;
            try
            {
                dto = context.Message;
                var product = new Product { Name = dto.Name, Price = dto.Price };
                var result = await _productService.CreateProductAsync(product);
                if (result <= 0)
                {
                    await context.Publish<ITransactionCreated>(new TransactionCreated
                    {
                        Message = $"创建订单失败，订单号：{JsonSerializer.Serialize(product)}",
                        Success = false
                    });
                }
                else
                {
                    await context.Publish<ITransactionCreated>(new TransactionCreated
                    {
                        Message = $"创建订单成功，订单号：{product.Id}",
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
