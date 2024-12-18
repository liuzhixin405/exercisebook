using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 用户提现地址管理表
    /// </summary>
    public partial class BqMemberWalletAddress
    {
        public uint Id { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public uint FUserId { get; set; }
        /// <summary>
        /// 币种ID
        /// </summary>
        public byte CurrencyTypeId { get; set; }
        /// <summary>
        /// 钱包地址
        /// </summary>
        public string WalletAddress { get; set; } = null!;
        /// <summary>
        /// 钱包标签
        /// </summary>
        public string Name { get; set; } = null!;
        /// <summary>
        /// 是否为默认 (1:默认 0:非默认)
        /// </summary>
        public bool IsDefault { get; set; }
        public ulong Erc20 { get; set; }
        public ulong Trc20 { get; set; }
    }
}
