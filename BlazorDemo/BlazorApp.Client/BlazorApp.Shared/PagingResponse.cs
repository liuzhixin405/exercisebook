using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorApp.Shared
{
    public class PagingResponse<T> where T:class
    {
        public List<T> Items { get; set; }
        public MetaData MetaData { get; set; }
    }
}
