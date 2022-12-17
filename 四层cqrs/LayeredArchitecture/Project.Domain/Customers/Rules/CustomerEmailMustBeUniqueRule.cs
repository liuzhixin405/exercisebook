using Project.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.Customers.Rules
{
    internal class CustomerEmailMustBeUniqueRule : IBusinessRule
    {
        private readonly ICustomerUniquenessChecker _customerUniquenessChecker;
        private readonly string _email;
        public string Message => "Customer with this email already exists.";

        public bool IsBroken() => !_customerUniquenessChecker.IsUnique(_email);
        public CustomerEmailMustBeUniqueRule(ICustomerUniquenessChecker customerUniquenessChecker,string email)
        {
            _customerUniquenessChecker= customerUniquenessChecker;
            _email= email;
        }
    }
}
