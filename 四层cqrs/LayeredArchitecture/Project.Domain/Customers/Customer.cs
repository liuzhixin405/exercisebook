using Project.Domain.Customers.Orders;
using Project.Domain.Customers.Orders.Events;
using Project.Domain.Customers.Rules;
using Project.Domain.ForeignExchange;
using Project.Domain.Products;
using Project.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.Customers
{
    public class Customer:Entity,IAggregateRoot
    {
        public CustomerId Id { get; private set; }

        private string _email;
        private string _name;
        private readonly List<Order> _orders;
        private bool _welcomeEmailWasSent;
        private Customer() { this._orders = new List<Order>(); }

        private Customer(string email, string name)
        {
            this.Id=new CustomerId(Guid.NewGuid());
            _email=email;
            _name=name;
            _welcomeEmailWasSent = false;
            _orders = new List<Order>();
            this.AddDomainEvent(new CustomerRegisteredEvent(this.Id));
        }
        public static Customer CreateRegistered(string email, string name,ICustomerUniquenessChecker customerUniquenessChecker)
        {
            CheckRule(new CustomerEmailMustBeUniqueRule(customerUniquenessChecker, null));
            return new Customer(email, name);
        }
        public OrderId PlaceOrder(List<OrderProductData> orderProductDatas,List<ProductPriceData> allProductPrices,string currency,List<ConversionRate> conversionRates)
        {
            CheckRule(new CustomerCannotOrderMoreThan2OrdersOnTheSameDayRule(_orders));
            CheckRule(new OrderMustHaveAtLeastOneProductRule(orderProductDatas));
            var order = Order.CreateNew(orderProductDatas, allProductPrices,currency, conversionRates);
            this._orders.Add(order);
            this.AddDomainEvent(new OrderPlacedEvent(order.Id, this.Id, order.GetValue()));
            return order.Id;
        }
        public void ChangeOrder(OrderId orderId, List<ProductPriceData> existingProdcuts, List<OrderProductData> newOrderProductsData,List<ConversionRate> conversionRates,string currency)
        {
            CheckRule(new OrderMustHaveAtLeastOneProductRule(newOrderProductsData));
            var order = this._orders.Single(x=>x.Id== orderId);
            order.Change(existingProdcuts, newOrderProductsData, conversionRates, currency);
            this.AddDomainEvent(new OrderChangedEvent(orderId));
        }
        
        public void RemoveOrder(OrderId orderId)
        {
            var order = this._orders.Single(x=>x.Id== orderId);
            order.Remove();
            this.AddDomainEvent(new OrderRemovedEvent(orderId));
        }
        public void MarkAsWelcomedByEmail() => this._welcomeEmailWasSent = true;
    }
}
