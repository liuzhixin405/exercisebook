using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWebApi.Server.Paging
{
    public static class PagedListExtensions
    {
        public static PagedList<TSource> ToPagedList<TSource>(this IList<TSource> source,int pageNumber,int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageNumber - 1) * pageSize)
                .Take(pageSize).ToList();
            return new PagedList<TSource>(items, count, pageNumber, pageSize);

        }
    }
}
