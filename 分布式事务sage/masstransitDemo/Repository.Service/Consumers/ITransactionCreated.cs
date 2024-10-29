using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Service.Consumers
{
    public interface ITransactionCreated
    {
        public bool Success { get; }
        public string Message { get;  }
    }
}
