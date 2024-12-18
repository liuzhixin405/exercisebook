using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// Ieo解锁设置
    /// </summary>
    public partial class BqIeoUnlockSetting
    {
        public int Id { get; set; }
        /// <summary>
        /// 解锁比例
        /// </summary>
        public decimal UnlockRate { get; set; }
        /// <summary>
        /// 解锁时间
        /// </summary>
        public int UnlockTime { get; set; }
        /// <summary>
        /// 私募时间区间开始（大于该时间的将按解锁比例释放）
        /// </summary>
        public int LockTimeBegin { get; set; }
        /// <summary>
        /// 私募时间区间结束（小于该时间的将按解锁比例释放）
        /// </summary>
        public int LockTimeEnd { get; set; }
    }
}
