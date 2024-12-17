using System;
using System.Collections.Generic;

namespace Project.Discuss.Models;

/// <summary>
/// 用户屏蔽列表
/// </summary>
public partial class BbsUserblock
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// 被屏蔽用户ID
    /// </summary>
    public long BlockedUserId { get; set; }

    /// <summary>
    /// 屏蔽原因
    /// </summary>
    public string? BlockReason { get; set; }

    /// <summary>
    /// 屏蔽状态（1-开启，0-关闭）
    /// </summary>
    public short BlockStatus { get; set; }

    /// <summary>
    /// 屏蔽时间
    /// </summary>
    public DateTime BlockTime { get; set; }
}
