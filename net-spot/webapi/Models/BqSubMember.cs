using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 母子账户关系表
    /// </summary>
    public partial class BqSubMember
    {
        public int Id { get; set; }
        /// <summary>
        /// 母账户id
        /// </summary>
        public int MotherUserId { get; set; }
        /// <summary>
        /// 子账号id
        /// </summary>
        public int SubUserId { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public int Dateline { get; set; }
    }
}
