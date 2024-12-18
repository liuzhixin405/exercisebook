using System;
using System.Collections.Generic;

namespace webapi
{
    public partial class BqSuggest
    {
        public uint Id { get; set; }
        /// <summary>
        /// 建议用户ID
        /// </summary>
        public int FUserId { get; set; }
        /// <summary>
        /// 建议用户邮箱
        /// </summary>
        public string FUserEmail { get; set; } = null!;
        /// <summary>
        /// 类型 1.页面错误 2. 逻辑错误 3 Bug 4其他
        /// </summary>
        public bool FSugType { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string FSugContent { get; set; } = null!;
        /// <summary>
        /// 提交时间
        /// </summary>
        public int FSugTime { get; set; }
        /// <summary>
        /// 状态 0 未处理 1 采纳 2拒绝
        /// </summary>
        public short FSugState { get; set; }
        /// <summary>
        /// 采纳回复
        /// </summary>
        public string FSugReply { get; set; } = null!;
        /// <summary>
        /// 回复时间
        /// </summary>
        public int FSugEndtime { get; set; }
    }
}
