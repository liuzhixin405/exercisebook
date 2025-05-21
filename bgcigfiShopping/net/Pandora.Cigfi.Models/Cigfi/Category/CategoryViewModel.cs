using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandora.Cigfi.Models.Cigfi
{
    public class CategoryViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long ParentId { get; set; }
        public int SortOrder { get; set; }
    }
}
