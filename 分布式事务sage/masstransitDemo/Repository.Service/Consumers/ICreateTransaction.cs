using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Service.Consumers
{
    public interface ICreateTransaction
    {
        public string CustomerName { get;  }
        public decimal TotalAmount { get;  }
        public string Name { get;  }
        public decimal Price { get;  }
    }
}
