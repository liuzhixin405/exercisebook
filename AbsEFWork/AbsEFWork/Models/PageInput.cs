namespace AbsEFWork.Models
{
    /// <summary>
    /// 分页查询基类
    /// </summary>
    public class PageInput
    {
        private string _sortType { get; set; } = "asc";

        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// 每页行数
        /// </summary>
        public int PageRows { get; set; } = int.MaxValue;

        /// <summary>
        /// 排序列
        /// </summary>
        public string SortField { get; set; } = "Id";

        /// <summary>
        /// 排序类型
        /// </summary>
        public string SortType { get => _sortType; set => _sortType = (value ?? string.Empty).ToLower().Contains("desc") ? "desc" : "asc"; }
    }

    public enum SortSide { Desc, Asc }

    public class PageInput<T> : PageInput where T : new()
    {
        public T Search { get; set; } = new T();
        public string GetCacheKey()
        {
            return $"{PageIndex}_{PageRows}_{SortField}_{SortType}_{(Search as ICacheKey)?.GetCacheKey()}";
        }
    }

    public interface ICacheKey
    {
        string GetCacheKey();
    }
}
