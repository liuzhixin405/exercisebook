using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 积分类别表,解决积分规则(一种可以多次添加积分，另一种是只允许添加一次积分)
    /// </summary>
    public partial class BqCommonCreditType
    {
        public byte CreditTypeId { get; set; }
        /// <summary>
        /// 积分类别名称
        /// </summary>
        public string CreditTypeName { get; set; } = null!;
        /// <summary>
        /// 最多奖励次数 (0:表示不限制 &gt;0:同一个积分类型最多奖励次数）
        /// </summary>
        public uint CreditLimitCount { get; set; }
    }
}
