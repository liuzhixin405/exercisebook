using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 合约介绍详情
    /// </summary>
    public partial class BqTransactionIntroduce
    {
        public int Id { get; set; }
        /// <summary>
        /// 合约号
        /// </summary>
        public string TransactionCode { get; set; } = null!;
        /// <summary>
        /// 合约类型：1、正式盘，2、模拟盘
        /// </summary>
        public short Type { get; set; }
        /// <summary>
        /// 分区
        /// </summary>
        public string Part { get; set; } = null!;
        /// <summary>
        /// 英文介绍
        /// </summary>
        public string? EnIntroduce { get; set; }
        /// <summary>
        /// 日文介绍
        /// </summary>
        public string? JaIntroduce { get; set; }
        /// <summary>
        /// 中文介绍
        /// </summary>
        public string? ZhIntroduce { get; set; }
        /// <summary>
        /// 韩文介绍
        /// </summary>
        public string? KoIntroduce { get; set; }
        /// <summary>
        /// 繁体介绍
        /// </summary>
        public string? HkIntroduce { get; set; }
    }
}
