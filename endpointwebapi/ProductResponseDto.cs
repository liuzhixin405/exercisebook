using System.Diagnostics.Eventing.Reader;

namespace webapi
{
    public class ProductResponseDto<T>
    {
        public int TotalCount { get; set; }
        public T Data { get; set; }
    }
}
