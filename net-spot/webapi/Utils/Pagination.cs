namespace webapi.Utils
{
    public class Pagination<T>
    {
        public int PageNumber { get; }
        public int PageSize { get; }
        public int TotalPages { get; }
        public int TotalItems { get; }
        public IEnumerable<T> Items { get; }

        public Pagination(IEnumerable<T> source, int pageNumber, int pageSize)
        {
           
            TotalItems = source.Count();           
            PageSize = pageSize > 0 ? pageSize : throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero.");
            TotalPages = (int)Math.Ceiling(TotalItems / (double)PageSize);
            PageNumber = pageNumber > 0 && pageNumber <= TotalPages ? pageNumber : 1;
            Items = source;
        }
    }
}
