using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandora.Cigfi.Models.Cigfi.Invitation
{
    public class CigfiMember
    {
        /// <summary>
        /// 会员ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 钱包地址
        /// </summary>
        public string WalletAddress { get; set; }

        /// <summary>
        /// 是否VIP会员
        /// </summary>
        public bool IsVip { get; set; }

        /// <summary>
        /// 邀请码
        /// </summary>
        public string? InviteCode { get; set; }

        /// <summary>
        /// 邀请人会员ID
        /// </summary>
        public long? InvitedBy { get; set; }

        /// <summary>
        /// 累计返佣金额
        /// </summary>
        public decimal RebateAmount { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
