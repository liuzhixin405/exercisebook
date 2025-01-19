using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 用户提现/充值记录,主要记录用户金额变更(个人财务流水）
    /// </summary>
    public partial class BqAccountAmountLog
    {
        public long LogId { get; set; }
        /// <summary>
        /// 描述标题
        /// </summary>
        public string Title { get; set; } = null!;
        /// <summary>
        /// 币种（btc/ltc,gtc)1:btc 2:ltc 3...(后期可添加币种)
        /// </summary>
        public byte CurrencyTypeId { get; set; }
        /// <summary>
        /// 操作类型(0:其它/1:充值/2:提现/3:交易/4:追加保证金/5:购买积分)
        /// </summary>
        public byte Operation { get; set; }
        /// <summary>
        /// 1:加款 -1:减款 
        /// </summary>
        public bool Direction { get; set; }
        /// <summary>
        /// 当前账户余额(bq_acount:btc/bq_account:ltc)
        /// </summary>
        public decimal CurrentAmount { get; set; }
        /// <summary>
        /// 增减
        /// </summary>
        public decimal ChangeAmount { get; set; }
        /// <summary>
        /// 变更后的金额
        /// </summary>
        public decimal LastestAmount { get; set; }
        /// <summary>
        /// 操作时间,unix时间格式
        /// </summary>
        public uint Dateline { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public uint FUserId { get; set; }
        /// <summary>
        /// 交易流水号
        /// </summary>
        public string TransNo { get; set; } = null!;
        /// <summary>
        /// 描述
        /// </summary>
        public string TransType { get; set; } = null!;
        /// <summary>
        /// 交易分表id
        /// </summary>
        public uint RecordId { get; set; }
        /// <summary>
        /// 变更前的冻结
        /// </summary>
        public decimal BeforeFrozen { get; set; }
        /// <summary>
        /// 变更后的冻结
        /// </summary>
        public decimal AfterFrozen { get; set; }
    }
}
