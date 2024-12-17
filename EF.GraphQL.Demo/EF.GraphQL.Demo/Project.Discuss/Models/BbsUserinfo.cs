using System;
using System.Collections.Generic;

namespace Project.Discuss.Models;

/// <summary>
/// 社区用户信息
/// </summary>
public partial class BbsUserinfo
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// 用户昵称
    /// </summary>
    public string UserName { get; set; } = null!;

    /// <summary>
    /// 头像
    /// </summary>
    public string Avatar { get; set; } = null!;

    /// <summary>
    /// 用户类型(0：普通用户，1：项目方，2：交易所，4：媒体，6：微博等,7:推特 ， 8：自媒体，16：个人，32:官方,64:钱包)
    /// </summary>
    public short UserType { get; set; }

    /// <summary>
    /// 用户等级
    /// </summary>
    public short UserLevel { get; set; }

    /// <summary>
    /// 用户认证类型
    /// </summary>
    public short UserCertType { get; set; }

    /// <summary>
    /// 用户状态（1-激活，0-关闭）
    /// </summary>
    public short UserStatus { get; set; }

    /// <summary>
    /// 禁言时间
    /// </summary>
    public DateTime MuteTime { get; set; }

    /// <summary>
    /// 解除禁言时间
    /// </summary>
    public DateTime ReleaseMuteTime { get; set; }

    /// <summary>
    /// 用户粉丝数
    /// </summary>
    public int Followers { get; set; }

    /// <summary>
    /// 用户关注数
    /// </summary>
    public int Follows { get; set; }

    /// <summary>
    /// 用户文章数
    /// </summary>
    public int Articles { get; set; }

    /// <summary>
    /// 用户被举报评论数
    /// </summary>
    public int ReportComments { get; set; }

    /// <summary>
    /// 用户评论数
    /// </summary>
    public int Comments { get; set; }

    /// <summary>
    /// 数据同步时间
    /// </summary>
    public DateTime ModifyTime { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 手机号码
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 手机区号
    /// </summary>
    public string? ContryAreaCode { get; set; }

    /// <summary>
    /// 被举报文章数
    /// </summary>
    public int ReportCount { get; set; }

    /// <summary>
    /// 是否显示V标志：1-是，0-不是
    /// </summary>
    public sbyte ShowV { get; set; }

    /// <summary>
    /// 权限，0：无权限，1：有发文权限，2：有发观点评论权限 3:有全部权限
    /// </summary>
    public sbyte Power { get; set; }

    /// <summary>
    /// 简介
    /// </summary>
    public string Introduction { get; set; } = null!;

    /// <summary>
    /// 邮箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 分享数
    /// </summary>
    public int? Shares { get; set; }

    /// <summary>
    /// 转发量
    /// </summary>
    public int? Forwardings { get; set; }

    /// <summary>
    /// 点赞数
    /// </summary>
    public int? Likes { get; set; }

    /// <summary>
    /// 用户信息背景图
    /// </summary>
    public string? BackgroundImage { get; set; }

    public virtual ICollection<BbsUserNotification> BbsUserNotificationSenders { get; set; } = new List<BbsUserNotification>();

    public virtual ICollection<BbsUserNotification> BbsUserNotificationUsers { get; set; } = new List<BbsUserNotification>();
}
