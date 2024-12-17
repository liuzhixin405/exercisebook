using System;
using System.Collections.Generic;

namespace Project.Discuss.Models;

/// <summary>
/// 用户禁言明细
/// </summary>
public partial class BbsMutedetail
{
    /// <summary>
    /// 禁言明细ID
    /// </summary>
    public long MuteDetailId { get; set; }

    /// <summary>
    /// 用户ID
    /// </summary>
    public long MuteUserId { get; set; }

    /// <summary>
    /// 禁言原因
    /// </summary>
    public string MuteReason { get; set; } = null!;

    /// <summary>
    /// 禁言时间
    /// </summary>
    public DateTime MuteTime { get; set; }

    /// <summary>
    /// 解除禁言时间
    /// </summary>
    public DateTime ReleaseMuteTime { get; set; }

    /// <summary>
    /// 禁言说明
    /// </summary>
    public string? Description { get; set; }
}
