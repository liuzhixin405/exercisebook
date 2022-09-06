using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorApp.Shared
{
    public class UserParameters
    {
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        public int _pageSize = 4;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
        public string SearchTerm { get; set; }
    }
}
