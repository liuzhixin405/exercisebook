using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 合约分区设置
    /// </summary>
    public partial class DeliveryTransactionPartition
    {
        public int Id { get; set; }
        /// <summary>
        /// 分区中文名称
        /// </summary>
        public string NameCn { get; set; } = null!;
        /// <summary>
        /// 分区英文名称
        /// </summary>
        public string NameEn { get; set; } = null!;
        /// <summary>
        /// 分区日文名称
        /// </summary>
        public string NameJp { get; set; } = null!;
        /// <summary>
        /// 韩文名称
        /// </summary>
        public string? NameKr { get; set; }
        /// <summary>
        /// 繁体名称
        /// </summary>
        public string? NameHk { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public short Sort { get; set; }
        /// <summary>
        /// 合约列表（逗号分隔）
        /// </summary>
        public string Codes { get; set; } = null!;
    }
}
