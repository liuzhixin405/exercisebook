using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Contract.Events
{
    public interface CustomerCreatedEvent
    {
        public int CustomerId { get; set; }
        public string FullName { get; set; }
    }
}
