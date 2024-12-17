using System;
using System.Collections.Generic;

namespace Project.Discuss.Models;

/// <summary>
/// 文章点赞
/// </summary>
public partial class BbsArticleLike
{
    public ulong LikeId { get; set; }

    /// <summary>
    /// 文章ID
    /// </summary>
    public long ArticleId { get; set; }

    /// <summary>
    /// 点赞用户ID
    /// </summary>
    public long LikeUserId { get; set; }

    /// <summary>
    /// 点赞状态(1-点赞，0-取消点赞)
    /// </summary>
    public ulong LikeStatus { get; set; }

    /// <summary>
    /// 点赞时间
    /// </summary>
    public DateTime LikeTime { get; set; }

    /// <summary>
    /// 取消点赞时间
    /// </summary>
    public DateTime? UnLikeTime { get; set; }

    /// <summary>
    /// 是否未读：1-是，0-不是
    /// </summary>
    public ulong UnRead { get; set; }
}
