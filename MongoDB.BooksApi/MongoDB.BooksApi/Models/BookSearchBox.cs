using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MongoDB.BooksApi.Models
{
    public class BookSearchBox: BaseSearch
    {
        public string Name { get; set; }
        public string Category { get; set; }

        public string Author { get; set; }
    }
}
