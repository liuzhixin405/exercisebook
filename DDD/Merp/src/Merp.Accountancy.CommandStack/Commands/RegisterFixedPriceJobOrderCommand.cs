using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merp.Infrastructure;

namespace Merp.Accountancy.CommandStack.Commands
{
    public sealed class RegisterFixedPriceJobOrderCommand : Command
    {
        public Guid CustomerId { get; private set; }
        public string CustomerName { get; private set; }
        public Guid ManagerId { get; private set; }
        public string ManagerName { get; private set; }
        public decimal Price { get; private set; }
        public DateTime DateOfStart { get; private set; }
        public DateTime DueDate { get; private set; }
        public string JobOrderName { get; private set; }
        public string PurchaseOrderNumber { get; private set; }
        public string Description { get; private set; }

        public RegisterFixedPriceJobOrderCommand(Guid customerId, string customerName, Guid managerId, string managerName, decimal price, DateTime dateOfStart, DateTime dueDate, string jobOrderName, string purchaseOrderNumber, string description)
        {
            CustomerId = customerId;
            CustomerName = customerName;
            ManagerId = managerId;
            ManagerName = managerName;
            Price = price;
            DateOfStart = dateOfStart;
            DueDate = dueDate;
            JobOrderName = jobOrderName;
            PurchaseOrderNumber = purchaseOrderNumber;
            Description = description;
        }
    }
}
