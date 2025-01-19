using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 用户钱包表(新用户时时，系统自动从钱包队列表bq_wallet_queue_{btc}中读取一个钱包地址，与其绑定，将记录到此表中，将队列表中的钱包记录删除。一个用户每种币种(btc/ltc)对应一条记录
    /// </summary>
    public partial class BqMemberWallet
    {
        public ulong Id { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public uint FUserId { get; set; }
        /// <summary>
        /// 货币类型
        /// </summary>
        public int CurrencyTypeId { get; set; }
        /// <summary>
        /// 钱包地址
        /// </summary>
        public string WalletAddress { get; set; } = null!;
        /// <summary>
        /// 钱包二维码图片地址(路径进行hash算法存储)
        /// </summary>
        public string WalletAddressPic { get; set; } = null!;
        public ulong Erc20 { get; set; }
        public ulong Trc20 { get; set; }
    }
}
