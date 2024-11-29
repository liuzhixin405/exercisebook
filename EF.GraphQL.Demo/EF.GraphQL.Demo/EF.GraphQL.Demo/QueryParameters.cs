﻿namespace EF.GraphQL.Demo
{
    public class QueryParameters
    {
        public int PageIndex { get; set; } = 1; // 默认第一页
        public int PageSize { get; set; } = 10; // 默认每页10条
        public string? OrderBy { get; set; } // 排序字段
        public bool IsDescending { get; set; } = false; // 是否降序
        //public Dictionary<string, object?> Filters { get; set; } = new(); // 动态过滤条件
        public List<CusFilter> Filters { get; set; } = new(); // 筛选条件
    }

    public class CusFilter
    {
        public string FieldName { get; set; }
        public object Value { get; set; }
        public string Operator { get; set; }
    }
}
