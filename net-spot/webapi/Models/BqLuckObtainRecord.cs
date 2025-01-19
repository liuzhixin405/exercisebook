using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 获奖记录表
    /// </summary>
    public partial class BqLuckObtainRecord
    {
        public int Id { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 用户名(4984****45@qq.com)
        /// </summary>
        public string UserName { get; set; } = null!;
        /// <summary>
        /// 奖项id
        /// </summary>
        public int PrizeId { get; set; }
        /// <summary>
        /// 奖项中文名称
        /// </summary>
        public string PrizeNameCn { get; set; } = null!;
        /// <summary>
        /// 奖项英文名称
        /// </summary>
        public string PrizeNameUs { get; set; } = null!;
        /// <summary>
        /// 实物奖励
        /// </summary>
        public string PrizeMatter { get; set; } = null!;
        /// <summary>
        /// usdt奖励
        /// </summary>
        public decimal PrizeUsdt { get; set; }
        /// <summary>
        /// 获奖时间
        /// </summary>
        public long ObtainTime { get; set; }
        /// <summary>
        /// 0，随机抽得；1，管理员指定抽得
        /// </summary>
        public ulong Random { get; set; }
        /// <summary>
        /// 奖品说明
        /// </summary>
        public string Notes { get; set; } = null!;
    }
}
