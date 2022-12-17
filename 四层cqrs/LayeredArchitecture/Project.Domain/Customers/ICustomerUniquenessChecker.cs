using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.Customers
{
    public interface ICustomerUniquenessChecker
    {
        bool IsUnique(String customerEmail);
    }
}
