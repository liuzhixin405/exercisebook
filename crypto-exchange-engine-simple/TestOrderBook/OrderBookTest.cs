using crypto_exchange_engine_simple.Engine;
using NUnit.Framework;
using System.Threading.Tasks;
namespace TestOrderBook
{
    public class Tests
    {
        OrderBook orderBook;
        [SetUp]
        public void SetUp()
        {

            var sellerOrders = new Order[] { new Order(1, 1, "1", 1), new Order(5, 5, "5", 5), new Order(3, 3, "3", 3) };
            var buyerOrders = new Order[] { new Order(1, 1, "1", 1), new Order(2, 2, "2", 2), new Order(3, 3, "3", 3) };
            orderBook = new OrderBook(sellerOrders, buyerOrders);
        }
        [Test]
        public Task TestRemoveTest()
        {
            int rmIndex = 2;
            orderBook.RemoveBuyOrder(rmIndex);
            orderBook.RemoveSellOrder(rmIndex);
            Assert.True(true);
            return Task.CompletedTask;
        }

        [Test]
        public Task TestAddTest()
        {
            orderBook.AddSellOrder(new Order(2,2,"2",2));
            return Task.CompletedTask;
        }
    }
}