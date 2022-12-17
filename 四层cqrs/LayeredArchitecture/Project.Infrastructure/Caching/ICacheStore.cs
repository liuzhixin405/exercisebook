using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.Caching
{
    public interface ICacheStore
    {
        void Add<TItem>(TItem item,ICacheKey<TItem> key,TimeSpan? expirationtime=null);
        void Add<TItem>(TItem item, ICacheKey<TItem> key, DateTime? absoluteExpiration = null);
        TItem Get<TItem>(ICacheKey<TItem> key) where TItem:class;
        void Remove<TItem>(ICacheKey<TItem> key);
    }
}
