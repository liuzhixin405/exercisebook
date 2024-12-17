using System;
using System.Collections.Generic;

namespace Project.Discuss.Models;

/// <summary>
/// 文章转发关联表
/// </summary>
public partial class BbsArticlesRelay
{
    /// <summary>
    /// 文章id
    /// </summary>
    public long ArticleId { get; set; }

    /// <summary>
    /// 用户的id
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// 被转发文章的id
    /// </summary>
    public long ParentArticleId { get; set; }

    /// <summary>
    /// 被转发文章的用户id
    /// </summary>
    public long ParentArticleUserId { get; set; }

    /// <summary>
    /// 转发时间
    /// </summary>
    public DateTime CreateTime { get; set; }
}
