using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 用户“充值/提现&quot;表
    /// </summary>
    public partial class BqMemberRecharge
    {
        public uint RecordId { get; set; }
        /// <summary>
        /// 币种（btc/ltc,gtc)1:btc 2:ltc 3...(后期可添加币种)
        /// </summary>
        public short CurrencyTypeId { get; set; }
        /// <summary>
        /// 操作类型(1:充值/2:提现)
        /// </summary>
        public short Operation { get; set; }
        /// <summary>
        /// (充值/提现)金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 手续费(只在提现的时候有)
        /// </summary>
        public decimal Fee { get; set; }
        /// <summary>
        /// 1.普通充值, 2.在线充值,3、奖励
        /// </summary>
        public short ReceiceCardType { get; set; }
        /// <summary>
        /// 接收卡号(比特币、莱特币钱包地址)
        /// </summary>
        public string ReceiceCardNo { get; set; } = null!;
        /// <summary>
        /// 申请地址
        /// </summary>
        public string Ip { get; set; } = null!;
        /// <summary>
        /// 申请时间
        /// </summary>
        public uint Dateline { get; set; }
        /// <summary>
        /// 确认时间(充值的时候，需要人工确认后，再手动加款）
        /// </summary>
        public uint ConfirmTime { get; set; }
        /// <summary>
        /// 0:未受理 1:受理成功 2:拒绝
        /// </summary>
        public bool State { get; set; }
        /// <summary>
        /// 备注说明
        /// </summary>
        public string Remark { get; set; } = null!;
        /// <summary>
        /// 用户ID
        /// </summary>
        public uint FUserId { get; set; }
        /// <summary>
        /// 用户邮箱
        /// </summary>
        public string FUserEmail { get; set; } = null!;
        /// <summary>
        /// 钱包交易ID(确认到账)
        /// </summary>
        public string WalletTradeId { get; set; } = null!;
        public string? FUserName { get; set; }
        public ulong UsdtErc20 { get; set; }
        public ulong UsdtTrc20 { get; set; }
    }
}
