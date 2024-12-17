using System;
using System.Collections.Generic;

namespace Project.Discuss.Models;

/// <summary>
/// 用户收藏的文章
/// </summary>
public partial class BbsCollection
{
    /// <summary>
    /// 主键
    /// </summary>
    public ulong CollectionId { get; set; }

    /// <summary>
    /// 文章id
    /// </summary>
    public long ArticleId { get; set; }

    /// <summary>
    /// 用户id
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }
}
