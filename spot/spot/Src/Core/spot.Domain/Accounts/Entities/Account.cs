using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace spot.Domain.Accounts.Entities
{
    public class Account
    {
        [Key]
        [Column("id")]
        public required string Id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Column("created_at")]
        public long CreatedAt { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [Column("updated_at")]
        public long UpdatedAt { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [Column("user_id")]
        public required string UserId { get; set; }

        /// <summary>
        /// 货币代码
        /// </summary>
        [Column("currency")]
        public required string Currency { get; set; }

        /// <summary>
        /// 可用余额
        /// </summary>
        [Column("available")]
        public decimal Available { get; set; }

        /// <summary>
        /// 冻结余额
        /// </summary>
        [Column("hold")]
        public decimal Hold { get; set; }

        /// <summary>
        /// 计算总余额
        /// </summary>
        [NotMapped]
        public decimal Total => Available + Hold;
    }
}