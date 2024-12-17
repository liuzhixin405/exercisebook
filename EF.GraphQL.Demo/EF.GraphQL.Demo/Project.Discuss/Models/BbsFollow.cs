using System;
using System.Collections.Generic;

namespace Project.Discuss.Models;

/// <summary>
/// 用户关注
/// </summary>
public partial class BbsFollow
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// 关注者用户ID
    /// </summary>
    public long FollowerUserId { get; set; }

    /// <summary>
    /// 关注时间
    /// </summary>
    public DateTime FollowTime { get; set; }

    /// <summary>
    /// 是否互相关注
    /// </summary>
    public ulong IsFriend { get; set; }
}
