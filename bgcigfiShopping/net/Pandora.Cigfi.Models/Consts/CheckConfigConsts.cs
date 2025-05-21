using System;
using System.Collections.Generic;
using System.Text;

namespace Pandora.Cigfi.Models.Consts
{
    /// <summary>
    /// 输入验证
    /// </summary>
    public class CheckConfig
    {
        /// <summary>
        /// 输入统计时间
        /// </summary>
        public int StatisticsMinute { get; set; }
        /// <summary>
        /// 最大次数
        /// </summary>
        public const int MaxCount = 5;
        /// <summary>
        /// 锁定N分钟
        /// </summary>
        public const int LockMinute = 10;
        /// <summary>
        /// 统计同一个ip
        /// </summary>
        public bool IsSameIP { get; set; }
    }
}
