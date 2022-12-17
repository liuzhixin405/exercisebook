using Project.Domain.Customers.Orders;
using Project.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.Customers.Rules
{
    public class CustomerCannotOrderMoreThan2OrdersOnTheSameDayRule : IBusinessRule
    {
        private readonly IList<Order> _orders;
        public string Message => "You cannot order more than 2 orders on the same day.";

        public CustomerCannotOrderMoreThan2OrdersOnTheSameDayRule(IList<Order> orders)
        {
            _orders= orders;
        }
        public bool IsBroken()
        {
            return _orders.Count(x => x.IsOrderedToday()) >= 2;
        }
    }
}
