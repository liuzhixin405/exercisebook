using eapi.Models;
using eapi.RedisHelper;
using eapi.Repositories;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading;

namespace eapi.Service
{
    public class OrderService : IOrderService
    {
        private readonly IRepositoryWrapper repositoryWrapper;
        public OrderService(IRepositoryWrapper repositoryWrapper)
        {
            this.repositoryWrapper = repositoryWrapper;
        }

        public async Task Completed(int orderId)
        {
            var order = await repositoryWrapper.OrderRepository.GetById(orderId);
            if (order == null)
            {
                throw new Exception("订单号错误");
            }
            if (order.Status != OrderStatus.Shipment)
            {
                throw new Exception("处置错误");
            }
            order.Status = OrderStatus.Completed;
            order.ShipMentTime = DateTime.Now;
            await repositoryWrapper.OrderRepository.Update(order);
        }
       
        public async Task Create(string sku, int count)
        {
            SpinLock sl = new SpinLock();
            bool gotLock = false;
            #region MyRegion
            //using (var client = new ConnectionHelper().Conn())
            //{
            //    bool isLocked = client.Add<string>("DataLock:" + sku, sku, TimeSpan.FromMicroseconds(500));
            //    if (isLocked)
            //    {
            //        try
            //        {
            //            var getProductFromCache = client.Get<Product>($"product_{sku}");
            //            if(getProductFromCache == null)
            //            {
            //                var product = repositoryWrapper.ProductRepository.FindByCondition(x => x.Sku.Equals(sku)).ConfigureAwait(false).GetAwaiter().GetResult().SingleOrDefault();
            //                getProductFromCache = product;
            //                client.Set<Product>($"product_{sku}", getProductFromCache);
            //            }
            //            if (getProductFromCache == null || getProductFromCache.Count < count)
            //            {
            //                throw new Exception("库存不足");
            //            }
            //            else
            //            {
            //                getProductFromCache.Count -= count;
            //            }
            //            await repositoryWrapper.Trans(async () =>
            //            {
            //                await repositoryWrapper.OrderRepository.Create(Order.Create(sku, count));
            //                //throw new Exception("2"); //测试用
            //                await repositoryWrapper.ProductRepository.Update(getProductFromCache);
            //                 client.Set<Product>($"product_{sku}", getProductFromCache);
            //            });
            //        }
            //        catch
            //        {
            //            throw;
            //        }
            //        finally
            //        {
            //            client.Remove("DataLock:" + sku);
            //        }
            //    }
            //    else
            //    {
            //        Console.WriteLine($"失败: 没有拿到锁");
            //    }
            //}

            #endregion


            try
                    {
                sl.Enter(ref gotLock);
                    var product = repositoryWrapper.ProductRepository.FindByCondition(x => x.Sku.Equals(sku)).ConfigureAwait(false).GetAwaiter().GetResult().SingleOrDefault();
                           
                        
                        if (product == null || product.Count < count)
                        {
                            throw new Exception("库存不足");
                        }
                        else
                        {
                            product.Count -= count;
                        }
                        await repositoryWrapper.Trans(async () =>
                        {
                            await repositoryWrapper.OrderRepository.Create(Order.Create(sku, count));
                            //throw new Exception("2"); //测试用
                            await repositoryWrapper.ProductRepository.Update(product);
                           
                        });
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                if (gotLock)
                    sl.Exit();
            }
  
            
        }

        public async Task Rejected(int orderId)
        {
            var order = await repositoryWrapper.OrderRepository.GetById(orderId);
            if (order == null)
            {
                throw new Exception("订单号错误");
            }
            if (order.Status == OrderStatus.Completed)
            {
                throw new Exception("已完成无法拒收");
            }

            order.Status = OrderStatus.Rejected;
            order.RejectedTime = DateTime.Now;

            var product = (await repositoryWrapper.ProductRepository.FindByCondition(x => x.Sku.Equals(order.Sku))).SingleOrDefault();
            if (product == null)
            {
                throw new Exception("product号错误");
            }
            product.Count += order.Count;

            await repositoryWrapper.Trans(async () =>
            {
                await repositoryWrapper.OrderRepository.Update(order);
                await repositoryWrapper.ProductRepository.Update(product);
            });

        }

        public async Task Shipment(int orderId)
        {
            var order = await repositoryWrapper.OrderRepository.GetById(orderId);
            if (order == null)
            {
                throw new Exception("订单号错误");
            }
            if (order.Status != OrderStatus.Created)
            {
                throw new Exception("处置错误");
            }
            order.Status = OrderStatus.Shipment;
            order.ShipMentTime = DateTime.Now;
            await repositoryWrapper.OrderRepository.Update(order);
        }
    }
}
