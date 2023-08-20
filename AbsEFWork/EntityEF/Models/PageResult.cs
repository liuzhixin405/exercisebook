namespace EntityEF.Models
{
    public class PageResult<T>:AjaxResult<List<T>>
    {
        public int Total { get; set; }
    }

    public class AjaxResult<T> : AjaxResult
    {
        public T Data { get; set; }
    }
    /// <summary>
    /// Ajax请求结果
    /// </summary>
    public class AjaxResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; } = true;

        /// <summary>
        /// 错误代码
        /// </summary>
        public int ErrorCode { get; set; } = -1;

        /// <summary>
        /// 返回消息
        /// </summary>
        public string Msg { get; set; }
    }
}
