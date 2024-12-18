using System;
using System.Collections.Generic;

namespace webapi
{
    public partial class BqMemberApplyIdcard
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public uint FUserId { get; set; }
        /// <summary>
        /// 手持身份证照片
        /// </summary>
        public string Photo1 { get; set; } = null!;
        /// <summary>
        /// 身份证（正面）
        /// </summary>
        public string Photo2 { get; set; } = null!;
        /// <summary>
        /// 身份证（反面）
        /// </summary>
        public string Photo3 { get; set; } = null!;
    }
}
