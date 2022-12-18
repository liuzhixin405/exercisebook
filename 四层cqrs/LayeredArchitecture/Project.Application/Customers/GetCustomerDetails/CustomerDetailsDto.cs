using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Application.Customers.GetCustomerDetails
{
    public class CustomerDetailsDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string WelcomeEmailWasSent { get; set; }
    }
}
