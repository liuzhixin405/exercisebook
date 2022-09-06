using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MongoDB.BooksApi.Models
{
    public class BaseSearch
    {
        public int PageIndex { get;set; }

        public int PageSize { get; set; }
    }
}
