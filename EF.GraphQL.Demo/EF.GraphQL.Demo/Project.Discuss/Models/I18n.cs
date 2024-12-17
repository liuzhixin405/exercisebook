using System;
using System.Collections.Generic;

namespace Project.Discuss.Models;

/// <summary>
/// 国际化表
/// </summary>
public partial class I18n
{
    /// <summary>
    /// id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 消息模板 key
    /// </summary>
    public string MsgKey { get; set; } = null!;

    public string? Us { get; set; }

    public string? Cn { get; set; }

    public string? Jp { get; set; }

    public string? Tw { get; set; }

    public string? Hk { get; set; }
}
