using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lx.eshop.models.Paging
{
    public class PaginatedItemsViewModel<TEntity> where TEntity : class
    {
        public int PageIndex { get;private set; }
        public int PageSize { get;private set; }
        public long TotalCount { get; private set;}

        public IEnumerable<TEntity> Data { get; private set; }
        public PaginatedItemsViewModel(int pageIndex, int pageSize, long totalCount, IEnumerable<TEntity> data)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = totalCount;
            Data = data;
        }
    }
}
