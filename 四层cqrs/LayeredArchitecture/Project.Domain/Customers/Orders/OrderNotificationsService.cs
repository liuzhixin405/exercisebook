using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.Customers.Orders
{
    public static class OrderNotificationsService
    {
        public static string GetOrderEmailConfirmationDescription(OrderId orderId)
        {
            return $"Order number: {orderId.Value} placed ";
        }
    }
}
