using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 用户合约开仓限制
    /// </summary>
    public partial class DeliveryUserOpenLimit
    {
        public int Id { get; set; }
        /// <summary>
        /// 合约号
        /// </summary>
        public string FCode { get; set; } = null!;
        /// <summary>
        /// 用户id
        /// </summary>
        public int FUserId { get; set; }
        /// <summary>
        /// 最大开多仓
        /// </summary>
        public decimal MaxKdcNums { get; set; }
        /// <summary>
        /// 最大开空仓
        /// </summary>
        public decimal MaxKkcNums { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public int AddTime { get; set; }
    }
}
