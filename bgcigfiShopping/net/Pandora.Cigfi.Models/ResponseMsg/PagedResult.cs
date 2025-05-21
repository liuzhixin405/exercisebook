using System.Collections.Generic;

namespace Pandora.Cigfi.Models.ResponseMsg
{
    public class PagedResult<T>
    {
        public int Total { get; set; }
        public List<T> Items { get; set; }
    }
}
