using System;
using System.Collections.Generic;

namespace Project.Discuss.Models;

/// <summary>
/// 文章分享
/// </summary>
public partial class BbsSharedetail
{
    /// <summary>
    /// 文章ID
    /// </summary>
    public long ArticleId { get; set; }

    /// <summary>
    /// 分享用户ID
    /// </summary>
    public long ShareUserId { get; set; }

    /// <summary>
    /// 分享目标
    /// </summary>
    public short ShareTarget { get; set; }

    /// <summary>
    /// 分享时间
    /// </summary>
    public DateTime ShareTime { get; set; }
}
