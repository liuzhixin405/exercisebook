using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 指定用户奖品表
    /// </summary>
    public partial class BqAppointLuck
    {
        public int Id { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 奖项
        /// </summary>
        public string Prize { get; set; } = null!;
        /// <summary>
        /// 抽取多少次获得
        /// </summary>
        public int Number { get; set; }
    }
}
