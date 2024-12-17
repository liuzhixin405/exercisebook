using System;
using System.Collections.Generic;

namespace Project.Discuss.Models;

/// <summary>
/// 文章参与话题列表
/// </summary>
public partial class BbsArticleTopic
{
    /// <summary>
    /// 文章ID
    /// </summary>
    public long ArticleId { get; set; }

    /// <summary>
    /// 话题ID
    /// </summary>
    public long TopicId { get; set; }

    /// <summary>
    /// 热度
    /// </summary>
    public decimal? Sort { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime? CreateTime { get; set; }

    /// <summary>
    /// 文章状态(-1-草稿,0-待审核，1-审核通过，2-下架，3-删除)
    /// </summary>
    public short? ArticleStatus { get; set; }
}
