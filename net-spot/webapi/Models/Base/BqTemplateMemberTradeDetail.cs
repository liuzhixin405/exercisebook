using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 交易详情表
    /// </summary>
    public partial class BqTemplateMemberTradeDetail
    {
        public uint Id { get; set; }
        /// <summary>
        /// 买方对应的交易ID
        /// </summary>
        public uint FBuyTradeId { get; set; }
        /// <summary>
        /// 买家uid
        /// </summary>
        public uint FBuyUid { get; set; }
        /// <summary>
        /// 买家用户邮箱
        /// </summary>
        public string FBuyUserEmail { get; set; } = null!;
        /// <summary>
        /// 卖家uid
        /// </summary>
        public uint FSellUid { get; set; }
        /// <summary>
        /// 卖家用户邮箱
        /// </summary>
        public string FSellUserEmail { get; set; } = null!;
        /// <summary>
        /// 卖方对应的交易id
        /// </summary>
        public uint FSellTradeId { get; set; }
        /// <summary>
        /// 成交类型ID(1:多单开仓/2:多单平仓/3:空头开仓/4:空头平仓，见_trade_log:trade_type_id)
        /// </summary>
        public string TradeBuyTypeId { get; set; } = null!;
        /// <summary>
        /// 同上
        /// </summary>
        public string TradeSellTypeId { get; set; } = null!;
        /// <summary>
        /// 交易类型标识(1:普通交易, 2:交割 3:强平)
        /// </summary>
        public string TradeFlag { get; set; } = null!;
        /// <summary>
        /// 这笔交易的成交方向(1:买 2:卖)
        /// </summary>
        public string TradeTransDirction { get; set; } = null!;
        /// <summary>
        /// 成交价格
        /// </summary>
        public decimal TradePrice { get; set; }
        /// <summary>
        /// 成交数量
        /// </summary>
        public decimal TradeCount { get; set; }
        /// <summary>
        /// 成交时间,unix时间格式
        /// </summary>
        public uint TradeTime { get; set; }
        /// <summary>
        /// 合约号id
        /// </summary>
        public ushort TransactionId { get; set; }
        /// <summary>
        /// 合约号
        /// </summary>
        public string TransactionCode { get; set; } = null!;
    }
}
