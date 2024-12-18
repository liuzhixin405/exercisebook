using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 用户安全设置项
    /// </summary>
    public partial class BqMemberSafeSetting
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public uint FUserId { get; set; }
        /// <summary>
        /// 用户安全类型(1短信验证，2google验证，3双重验证)
        /// </summary>
        public byte FUserSafetype { get; set; }
        /// <summary>
        /// 手机区号
        /// </summary>
        public string FUserZone { get; set; } = null!;
        /// <summary>
        /// 用户电话(手机/固话/海外电话),可能手机号认证
        /// </summary>
        public string FUserTel { get; set; } = null!;
        /// <summary>
        ///  只有f_user_safetype值为2,3时才有效
        /// </summary>
        public string FUserSafegoogle { get; set; } = null!;
        /// <summary>
        /// 用户密保(用密保问题的ID(1|3|4)),以json格式存储
        /// </summary>
        public byte FUserSafequertion { get; set; }
        /// <summary>
        /// 用户密保答案(用密保问题的答案(aa|ce|hh)),以json格式存储
        /// </summary>
        public string FUserSafeanswer { get; set; } = null!;
    }
}
