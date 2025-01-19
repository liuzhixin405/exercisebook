using System;
using System.Collections.Generic;

namespace webapi
{
    public partial class BqCommonDatum
    {
        public int Id { get; set; }
        /// <summary>
        /// 分割盈利
        /// </summary>
        public decimal Data { get; set; }
        /// <summary>
        /// 合约号
        /// </summary>
        public string Content { get; set; } = null!;
    }
}
