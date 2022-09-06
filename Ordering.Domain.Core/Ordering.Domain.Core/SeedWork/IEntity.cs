using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.Core.SeedWork
{
    internal interface IEntity<out TKey>
    {
        TKey Id { get; }
    }
}
