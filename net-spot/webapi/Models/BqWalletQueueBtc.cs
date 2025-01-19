using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 钱包存储队列，一量钱包被使用后，将从此表删除，转存储到bq_member_waller_btc(后期可以使用mq代码，只要一个address值就可以了)
    /// </summary>
    public partial class BqWalletQueueBtc
    {
        public long WalletId { get; set; }
        /// <summary>
        /// 钱包地址
        /// </summary>
        public string WalletAddress { get; set; } = null!;
        /// <summary>
        /// 图片地址
        /// </summary>
        public string WalletPic { get; set; } = null!;
        public short CurrencyTypeId { get; set; }
        public ulong Erc20 { get; set; }
        /// <summary>
        /// trc20地址
        /// </summary>
        public ulong Trc20 { get; set; }
        /// <summary>
        /// 状态（0-未使用，1-锁定）
        /// </summary>
        public short State { get; set; }
    }
}
