using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Service.Consumers
{
    public class TransactionCreated : ITransactionCreated
    {
        public bool Success { get; set; }

        public string Message { get; set; }
    }
}
