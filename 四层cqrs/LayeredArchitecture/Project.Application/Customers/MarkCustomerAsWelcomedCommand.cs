using MediatR;
using Project.Application.Configuration.Commands;
using Project.Domain.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Project.Application.Customers
{
    public class MarkCustomerAsWelcomedCommand:InternalCommandBase<Unit>
    {
        [JsonConstructor]
        public MarkCustomerAsWelcomedCommand(Guid id,CustomerId customerId):base(id)
        {
            CustomerId = customerId;
        }
        public CustomerId CustomerId { get; }
    }
}
