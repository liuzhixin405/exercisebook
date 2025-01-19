using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 平款及各个已交割合约的手续费金额
    /// </summary>
    public partial class BqCountAccount
    {
        public int FId { get; set; }
        /// <summary>
        /// 币数
        /// </summary>
        public decimal? FAmount { get; set; }
        /// <summary>
        /// 1 平帐 2 已交割手续费
        /// </summary>
        public short? FType { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string? FContent { get; set; }
        public int? FTime { get; set; }
        /// <summary>
        /// 合约号
        /// </summary>
        public string? FTransCode { get; set; }
    }
}
