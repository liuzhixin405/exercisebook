using Project.Domain.ForeignExchange;
using Project.Domain.Products;
using Project.Domain.SeedWork;
using Project.Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.Customers.Orders
{
    public class Order:Entity
    {
        internal OrderId Id;
        private bool _isRemoved;
        private MoneyValue _value;
        private MoneyValue _valueInEUR;

        private List<OrderProduct> _orderProducts;
        private OrderStatus _status;
        private DateTime _orderDate;
        private DateTime? _orderChangeDate;
        public Order()
        {
            this._orderProducts = new List<OrderProduct>(); 
            this._isRemoved= false;
        }
        private Order(List<OrderProductData> orderProductsData,
            List<ProductPriceData> productPrices,
            string currency,
            List<ConversionRate> conversionRates)
        {
            this._orderDate = SystemClock.Now;
            this.Id = new OrderId(Guid.NewGuid());
            this._orderProducts = new List<OrderProduct>();

            foreach (var orderProductData in orderProductsData)
            {
                var productPrice = productPrices.Single(x => x.ProductId == orderProductData.ProductId && x.Price.Currency == currency);
                var orderProduct = OrderProduct.CreateForProduct(productPrice, orderProductData.Quantity, currency, conversionRates);
                _orderProducts.Add(orderProduct);   
            }
            this.CalculateOrderValue();
            this._status =  OrderStatus.Placed;
        }
        internal static Order CreateNew(List<OrderProductData> orderProductsData,List<ProductPriceData> allPrductPrices,string currency,List<ConversionRate> conversionRates)
        {
            return new Order(orderProductsData,allPrductPrices, currency, conversionRates);
        }
        internal void Change(List<ProductPriceData> allPrductPrices, List<OrderProductData> orderProductsData,  List<ConversionRate> conversionRates, string currency)
        {
            foreach (var orderProductData in orderProductsData)
            {
                var product = allPrductPrices.Single(x => x.ProductId == orderProductData.ProductId && x.Price.Currency == currency);
                var existingProductOrder = _orderProducts.Single(x => x.ProductId == orderProductData.ProductId);
                if(existingProductOrder != null)
                {
                    var existingOrderProduct = this._orderProducts.Single(x => x.ProductId == existingProductOrder.ProductId);
                    existingOrderProduct.ChangeQuantity(product, orderProductData.Quantity, conversionRates);
                }
                else
                {
                    var orderProduct = OrderProduct.CreateForProduct(product, orderProductData.Quantity, currency, conversionRates);
                    this._orderProducts.Add(orderProduct);
                }
            }
            var orderProductsToCheck = _orderProducts.ToList();
            foreach (var existingProduct in orderProductsToCheck)
            {
                var product = orderProductsData.SingleOrDefault(x => x.ProductId == existingProduct.ProductId);
                if(product==null)
                    this._orderProducts.Remove(existingProduct);
            }
            this.CalculateOrderValue();
            this._orderChangeDate = DateTime.UtcNow;
        }
        internal void Remove()
            => _isRemoved = true;
        internal bool IsOrderedToday()
        {
            return this._orderDate.Date == SystemClock.Now.Date;
        }
        internal MoneyValue GetValue()
        {
            return _value;
        }
        private void CalculateOrderValue()
        {
            _value = _orderProducts.Sum(x => x.Value);
            _valueInEUR = _orderProducts.Sum(x => x.ValueInEUR);
        }
    }
}
