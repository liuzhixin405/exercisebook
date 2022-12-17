using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.Caching
{
    public  interface ICacheKey<TItem>
    {
        string CacheKey { get; }
    }
}
