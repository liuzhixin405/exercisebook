namespace webapi
{
    public class PageInput
    {
        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// 每页行数
        /// </summary>
        public int PageRows { get; set; } = 100;

        /// <summary>
        /// 排序列
        /// </summary>
        public string SortField { get; set; } = "Id";

        /// <summary>
        /// 排序类型
        /// </summary>
        public bool Desc { get; set; } = true;
    }
    public class PageInput<T> :PageInput where T : struct
    {
        public T Search { get; set; } = new T();
    }
    
}
