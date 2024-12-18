using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 会员交易记录关系表(多对多),一个交易，需要将进行这个交易的双方与交易的对应关系都写入此表一份
    /// </summary>
    public partial class BtcusdtMemberTrade
    {
        /// <summary>
        /// 会员与交易对应关系ID
        /// </summary>
        public uint Id { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public uint FUserId { get; set; }
        /// <summary>
        /// 交易ID
        /// </summary>
        public uint TradeId { get; set; }
        /// <summary>
        /// 交易时间
        /// </summary>
        public uint Dateline { get; set; }
    }
}
