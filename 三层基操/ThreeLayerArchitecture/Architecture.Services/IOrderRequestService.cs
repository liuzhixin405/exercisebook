using Architecture.Domain.Customers;
using Architecture.Domain.Orders;
using Architecture.Domain.Products;
using Architecture.Domain.Shared;
using Architecture.Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Architecture.Services
{
    public interface IOrderRequestService
    {
        LowStockReport CheckStockLevelForOrderedItems(ShoppingCart cart);
        void RefillStoreForProduct(IEnumerable<Product> products);
        void SaveCheckoutInformation(ShoppingCart orderRequest, Address address, CreditCard card);
        bool CheckCustomerPaymentHistory(string customerId);
        int GenerateTemporaryOrderId();
    }
}
