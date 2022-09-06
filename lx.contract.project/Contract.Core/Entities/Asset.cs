using Contract.SharedKernel;
using Contract.SharedKernel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.Core.Entities
{
    public class Asset:EntityBase, IAggregateRoot
    {
        public decimal Balance { get; set; }

    }
}
