using System;
using System.Collections.Generic;

namespace Project.Discuss.Models;

/// <summary>
/// 用户申诉明细
/// </summary>
public partial class BbsUserinfoAppeal
{
    public int Id { get; set; }

    /// <summary>
    /// 处理状态，0:待处理，1：已处理,2:已禁言
    /// </summary>
    public sbyte State { get; set; }

    /// <summary>
    /// 用户id
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// 图片url数组
    /// </summary>
    public string Images { get; set; } = null!;

    /// <summary>
    /// 申诉内容
    /// </summary>
    public string Content { get; set; } = null!;

    public DateTime CreateTime { get; set; }

    public DateTime ModifyTime { get; set; }
}
