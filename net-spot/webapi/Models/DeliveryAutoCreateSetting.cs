using System;
using System.Collections.Generic;

namespace webapi
{
    public partial class DeliveryAutoCreateSetting
    {
        public int Id { get; set; }
        /// <summary>
        /// 交割类型
        /// </summary>
        public string DeliveryType { get; set; } = null!;
        /// <summary>
        /// 交割时间点
        /// </summary>
        public string EndTime { get; set; } = null!;
        public bool IsOpen { get; set; }
        public string Multiple { get; set; } = null!;
        public string MultipleState { get; set; } = null!;
        public decimal TradeCoefficient { get; set; }
        /// <summary>
        /// 合约种类
        /// </summary>
        public short TransactionKind { get; set; }
        /// <summary>
        /// 合约类型id
        /// </summary>
        public short TransactionTypeId { get; set; }
    }
}
