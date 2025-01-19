using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 持有平台币享VIP等级设置
    /// </summary>
    public partial class BqBfxVipSetting
    {
        public int Id { get; set; }
        /// <summary>
        /// 平台币持有量
        /// </summary>
        public decimal HoldCount { get; set; }
        /// <summary>
        /// vip等级id
        /// </summary>
        public short LevelId { get; set; }
        /// <summary>
        /// vip等级名称
        /// </summary>
        public string LevelName { get; set; } = null!;
        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; } = null!;
    }
}
