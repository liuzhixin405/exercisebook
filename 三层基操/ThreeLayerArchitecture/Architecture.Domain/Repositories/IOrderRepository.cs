using Architecture.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Architecture.Domain.Repositories
{
    public interface IOrderRepository:IRepository<Order>
    {
        Order FindById(int id);
        Order FindLastByCustomer(String customerId);
    }
}
