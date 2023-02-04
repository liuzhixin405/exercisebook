using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crypto_exchange_engine_simple.Engine
{
    public class OrderBook
    {
        public OrderBook(Order[] sellerOrders, Order[] buyerOrders)
        {
            this.sellerOrders = sellerOrders;
            this.buyerOrders = buyerOrders;
        }

         Order[] sellerOrders;
         Order[] buyerOrders;

        public Task<OrderBook> AddBuyOrder(Order order)
        {
            Order[] result;
            var n = buyerOrders.Length;
            var i = 0;
            for (i =n-1 ; i >=0; i--)
            {
                var buyOrder = buyerOrders[i];
                if (buyOrder.Price < order.Price)
                    break;
            }
            if (i == n - 1)
            {
                result = buyerOrders.Append(order).ToArray();
            }
            else if (i == -1)
            {
                result = buyerOrders.Prepend(order).ToArray();
            }
            else
            {
                result = new Order[n+1];
                Array.Copy(buyerOrders,result,n);
                
                for (int j = n; j > i; j--)
                {
                    result[j] = result[j - 1];
                }
                result[i] = order;
                
            }
            buyerOrders = result;
            return Task.FromResult(this);
        }

        public Task<OrderBook> AddSellOrder(Order order)
        {
            Order[] result;
            var n = sellerOrders.Length;
            var i = 0;
            for (i = n-1; i >= 0 ; i--)
            {
                var sellOrder = sellerOrders[i];
                if(sellOrder.Price > order.Price)
                {
                    break;
                }
            }
            if (i == n - 1)
            {
               result = sellerOrders.Append(order).ToArray();
            }
            else if (i == -1)
            {
               result = sellerOrders.Prepend(order).ToArray();
            }
            else
            {
                result = new Order[n+1];
                for (int j = n; j > i; j--)
                {
                    result[j] = result[j-1];
                }
                result[i]=order;
            }
            sellerOrders = result;
            return Task.FromResult(this);
        }

        public Task RemoveBuyOrder(int index)
        {
            Order[] newBuyOrder = new Order[buyerOrders.Length-1];
            var len = buyerOrders.Length;
            for (int i = index; i > len-1; i--)
            {
                buyerOrders[i] = buyerOrders[i+1];
            }
            Array.Copy(buyerOrders, newBuyOrder, newBuyOrder.Length);
            buyerOrders = newBuyOrder;
            return Task.CompletedTask;
        }
        public Task RemoveSellOrder(int index)
        {
            Order[] newSellOrder = new Order[sellerOrders.Length - 1];
            var len = sellerOrders.Length;
            for (int i = index; i > len - 1; i--)
            {
                sellerOrders[i] = sellerOrders[i + 1];
            }
            Array.Copy(sellerOrders, newSellOrder, newSellOrder.Length);
            sellerOrders = newSellOrder;
            return Task.CompletedTask;
        }

        public Task<Trade[]> Process(Order order)
        {
            if (order.Side == 1)
            {
                return ProcessLimitBuy(order);
            }
            return ProcessLimitSell(order);
        }

        public Task<Trade[]> ProcessLimitBuy(Order order)
        {
            var trades = new List<Trade>();
            var n = sellerOrders.Length;
            if (n != 0 || sellerOrders[n - 1].Price <= order.Price)
            {
                for (int i = n - 1; i >= 0; i--)
                {
                    var sellOrder = sellerOrders[i];
                    if (sellOrder.Price > order.Price)
                        break;
                    if (sellOrder.Amount >= order.Amount) 
                    {
                        trades.Add(new Trade { Amount = order.Amount, Price = sellOrder.Price, MakerOrderId = sellOrder.Id, TakerOrderId = order.Id }); //已撮合写入成交单
                        sellOrder.Amount -= order.Amount;
                        if (sellOrder.Amount == 0)
                        {
                            RemoveSellOrder(i);
                            Console.WriteLine($"已成交:{order.ToString()},对手id:{sellOrder.Id}");
                        }
                        return Task.FromResult(trades.ToArray());
                    }
                    if (sellOrder.Amount < order.Amount)  
                    {
                        trades.Add(new Trade
                        {
                            Amount = sellOrder.Amount,
                            Price = sellOrder.Price,
                            MakerOrderId = sellOrder.Id,
                            TakerOrderId = order.Id
                        });
                        order.Amount -= sellOrder.Amount;  
                        RemoveSellOrder(i);
                        Console.WriteLine($"已成交:{sellOrder.ToString()},对手id:{order.Id}");
                        continue;
                    }
                }
            }
            AddBuyOrder(order); 
            return Task.FromResult(trades.ToArray());
        }

        public Task<Trade[]> ProcessLimitSell(Order order)
        {
            var trades = new List<Trade>();
            var n = buyerOrders.Length;
            if(n!=0|| buyerOrders[n-1].Price >= order.Price)
            {
                for (int i = n-1; i >=0; i--)
                {
                    var buyerOrder = buyerOrders[i];
                    if(buyerOrder.Price < order.Price)
                    {
                        break;
                    }
                    if(buyerOrder.Amount >= order.Amount)
                    {
                        trades.Add(new Trade { TakerOrderId = order.Id, MakerOrderId = buyerOrder.Id, Amount = order.Amount, Price = buyerOrder.Price });
                        buyerOrder.Amount -= buyerOrder.Amount;
                        if (buyerOrder.Amount == 0)
                        {
                            RemoveBuyOrder(i);
                            Console.WriteLine($"已成交:{order.ToString()},对手id:{buyerOrder.Id}");
                        }
                        return Task.FromResult(trades.ToArray());
                    }
                    if(buyerOrder.Amount < order.Amount)
                    {
                        trades.Add(new Trade { TakerOrderId = order.Id, MakerOrderId = buyerOrder.Id, Amount = buyerOrder.Amount, Price = buyerOrder.Price });
                        order.Amount -= buyerOrder.Amount;
                        RemoveBuyOrder(i);
                        Console.WriteLine($"已成交:{buyerOrder.ToString()},对手id:{order.Id}");
                        continue;
                    }
                }
            }
            AddSellOrder(order);
            return Task.FromResult(trades.ToArray());
        }
    }
}
