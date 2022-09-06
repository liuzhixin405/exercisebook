using System;
using System.Collections.Generic;
using System.Text;

namespace SignService.Util.Primitives
{
    /// <summary>
    /// Ajax请求结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AjaxResult<T>:AjaxResult
    {
        /// <summary>
        /// 返回数据
        /// </summary>
        public T Data { get; set; }
    }
}
