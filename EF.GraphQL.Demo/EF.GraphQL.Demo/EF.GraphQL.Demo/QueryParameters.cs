namespace EF.GraphQL.Demo
{
    public class QueryParameters
    {
        public int PageIndex { get; set; } = 1; // 默认第一页
        public int PageSize { get; set; } = 10; // 默认每页10条
        public string? OrderBy { get; set; } // 排序字段
        public bool IsDescending { get; set; } = false; // 是否降序
        public Dictionary<string, object?> Filters { get; set; } = new(); // 动态过滤条件
    }
}
