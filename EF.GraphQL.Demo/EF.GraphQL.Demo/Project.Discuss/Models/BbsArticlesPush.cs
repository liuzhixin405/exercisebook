using System;
using System.Collections.Generic;

namespace Project.Discuss.Models;

/// <summary>
/// 早报推送记录
/// </summary>
public partial class BbsArticlesPush
{
    /// <summary>
    /// 文章ID
    /// </summary>
    public long ArticleId { get; set; }

    /// <summary>
    /// 1:已推送，0：待推送
    /// </summary>
    public int IsPush { get; set; }

    /// <summary>
    /// 添加时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 修改时间
    /// </summary>
    public DateTime UpdateTime { get; set; }
}
