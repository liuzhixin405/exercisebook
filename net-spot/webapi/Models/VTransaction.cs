using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 合约表（当前进行中的合约/已经清算完的合约/未开始的合约）
    /// </summary>
    public partial class VTransaction
    {
        public ushort TransactionId { get; set; }
        /// <summary>
        /// 合约号
        /// </summary>
        public string TransactionCode { get; set; } = null!;
        /// <summary>
        /// unix时间格式（旧合约清算时，自动写入新合约信息，将并时间自动调整为30分钟后的时间点）
        /// </summary>
        public uint TransactionBeginTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public uint TransactionEndTime { get; set; }
        /// <summary>
        /// 合约状态(1:正常 2:清算中 3:关闭)
        /// </summary>
        public bool TransactionState { get; set; }
        /// <summary>
        /// 最新成交价
        /// </summary>
        public decimal LatestTransactionPrice { get; set; }
        /// <summary>
        /// 最高报价
        /// </summary>
        public decimal TransactionHighest { get; set; }
        /// <summary>
        /// 最低报价
        /// </summary>
        public decimal TransactionLowest { get; set; }
        /// <summary>
        /// 合约排序值, YYMM  (1月 2日 3周 4季 5上证)
        /// </summary>
        public ushort Sort { get; set; }
        /// <summary>
        /// 具有的杠杆(1,5,10,20,50)
        /// </summary>
        public string Multiple { get; set; } = null!;
        /// <summary>
        /// 每个杠杆的状态(1,0,1,2,1)(0不显示,1显示,2禁用)
        /// </summary>
        public string MultipleState { get; set; } = null!;
        /// <summary>
        /// 交易系数(分母)
        /// </summary>
        public decimal TradeCoefficient { get; set; }
        /// <summary>
        /// 合约类型(1 btc合约 2 ltc合约 3sz合约)
        /// </summary>
        public short TransactionType { get; set; }
        /// <summary>
        /// 合约类型号
        /// </summary>
        public string TransactionTypeCode { get; set; } = null!;
        /// <summary>
        /// 是否暂停
        /// </summary>
        public ulong Suspend { get; set; }
        /// <summary>
        /// 合约可转入
        /// </summary>
        public ulong TransferInEnable { get; set; }
        /// <summary>
        /// 可约可转出
        /// </summary>
        public ulong TransferOutEnable { get; set; }
        /// <summary>
        /// 交易币种ID
        /// </summary>
        public int CurrencyTypeId { get; set; }
    }
}
