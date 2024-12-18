using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 用户基本信息表，支持邮箱登录,邮箱地址必须唯一(f_user_id为1表示系统用户)
    /// </summary>
    public partial class BqMember
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public uint FUserId { get; set; }
        /// <summary>
        /// 会员邮箱(支持登录)
        /// </summary>
        public string FUserEmail { get; set; } = null!;
        /// <summary>
        /// 邮箱认证(1:已认证 0:未认证)
        /// </summary>
        public bool FUserEmailStatus { get; set; }
        /// <summary>
        /// 密码(32位)
        /// </summary>
        public string FUserPwd { get; set; } = null!;
        /// <summary>
        /// 积分(活跃度)
        /// </summary>
        public ulong FUserCredits { get; set; }
        /// <summary>
        /// 用户VIP级别(bq_member_level:level_id)
        /// </summary>
        public byte FUserLevelId { get; set; }
        /// <summary>
        /// 用户状态（0:无效,1:有效,-1:冻结）
        /// </summary>
        public sbyte FUserState { get; set; }
        /// <summary>
        /// 密码随机码6位salt
        /// </summary>
        public string FUserSalt { get; set; } = null!;
        /// <summary>
        /// 注册时间,unix时间格式
        /// </summary>
        public uint FUserTime { get; set; }
        /// <summary>
        /// 用户真实邮箱
        /// </summary>
        public string? FRealEmail { get; set; }
        /// <summary>
        /// 用户是否开过仓
        /// </summary>
        public short HasOpenPosition { get; set; }
        /// <summary>
        /// 用户来源
        /// </summary>
        public string FUserOrigin { get; set; } = null!;
        /// <summary>
        /// bcex用户是否第一次登陆(0:否,1:是)
        /// </summary>
        public bool? FFirstLogin { get; set; }
        /// <summary>
        /// 手续费扣费方式（0-USDT，1-BFX）
        /// </summary>
        public short FFeeRateType { get; set; }
        /// <summary>
        /// 是否是子账号(0:否,1:是)
        /// </summary>
        public bool IsSubUser { get; set; }
        /// <summary>
        /// 子账号备注名
        /// </summary>
        public string SubUserNote { get; set; } = null!;
        /// <summary>
        /// 子账号是否删除(0:否,1:是)
        /// </summary>
        public bool SubUserDelete { get; set; }
        /// <summary>
        /// 是否开启会员管理(0:否,1:是)
        /// </summary>
        public bool OpenInviteManagement { get; set; }
        /// <summary>
        /// 用户昵称
        /// </summary>
        public string? FUserNickName { get; set; }
        public string? FUserHeadImg { get; set; }
        /// <summary>
        /// 用户身份(0:普通用户,1:代理人)
        /// </summary>
        public short FUserIdentity { get; set; }
        /// <summary>
        /// 用户是否属于代理系统(0:不属于,1:属于)
        /// </summary>
        public short FUserOms { get; set; }
    }
}
