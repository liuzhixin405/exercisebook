using System;
using System.Collections.Generic;

namespace Project.Discuss.Models;

/// <summary>
/// 用户浏览记录表
/// </summary>
public partial class BbsViewRecord
{
    /// <summary>
    /// 浏览记录 ID
    /// </summary>
    public long ViewId { get; set; }

    /// <summary>
    /// 用户 ID，关联用户表
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// 浏览内容 ID
    /// </summary>
    public long ContentId { get; set; }

    /// <summary>
    /// 内容类型（1-文章，2-评论，3-其他）
    /// </summary>
    public sbyte ContentType { get; set; }

    /// <summary>
    /// 浏览时间
    /// </summary>
    public DateTime ViewTime { get; set; }

    /// <summary>
    /// 用户 IP 地址
    /// </summary>
    public string? Ipaddress { get; set; }

    /// <summary>
    /// 用户设备信息
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// 浏览时长（秒）
    /// </summary>
    public int? Duration { get; set; }

    /// <summary>
    /// 来源链接
    /// </summary>
    public string? Referrer { get; set; }
}
