using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 用户积分类型统计表，每一个用户每一种积分类型对应一条记录，按类型统计.可以用在积分奖励功能(多次奖励积分和一次奖励积分)
    /// </summary>
    public partial class BqMemberCreditCount
    {
        public uint Id { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public uint FUserId { get; set; }
        /// <summary>
        /// 积分类型
        /// </summary>
        public byte CreditTypeId { get; set; }
        /// <summary>
        /// 积分次数(每积分一次+1),不包含扣除
        /// </summary>
        public uint CreditCount { get; set; }
        /// <summary>
        /// 总积分数量
        /// </summary>
        public uint Credits { get; set; }
    }
}
