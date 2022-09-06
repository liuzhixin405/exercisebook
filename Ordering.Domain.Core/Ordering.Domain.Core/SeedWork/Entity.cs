using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.Core.SeedWork
{
    internal abstract class Entity<TKey> : IEntity<TKey>
    {
        protected Entity(TKey id) => Id = id;
        public TKey Id { get; set; }
    }
}
