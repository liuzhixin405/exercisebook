using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merp.Accountancy.QueryStack.Model
{
    public abstract class JobOrder
    {
        public int Id { get; set; }
        public Guid OriginalId { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public Guid ManagerId { get; set; }
        public string ManagerName { get; set; }
        public DateTime DateOfStart { get; set; }
        public DateTime? DateOfCompletion { get; set; }
        public string Name { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsFixedPrice { get; set; }
        public bool IsTimeAndMaterial { get; set; }
        public string Description { get; set; }
        public string Number { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public JobOrder()
        {
            IsFixedPrice = false;
            IsTimeAndMaterial = false;
        }
    }
}
