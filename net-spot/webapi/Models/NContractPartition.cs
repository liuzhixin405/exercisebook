using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 合约分区表
    /// </summary>
    public partial class NContractPartition
    {
        public int Id { get; set; }
        /// <summary>
        /// 分区名称
        /// </summary>
        public string Part { get; set; } = null!;
        /// <summary>
        /// 币种Id
        /// </summary>
        public short CurrencyTypeId { get; set; }
    }
}
