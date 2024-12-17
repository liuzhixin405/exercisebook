using System;
using System.Collections.Generic;

namespace Project.Discuss.Models;

/// <summary>
/// 文章评论
/// </summary>
public partial class BbsComment
{
    /// <summary>
    /// 评论ID
    /// </summary>
    public long CommentId { get; set; }

    /// <summary>
    /// 评论者ID
    /// </summary>
    public long CommentUserId { get; set; }

    /// <summary>
    /// 文章ID
    /// </summary>
    public long ArticleId { get; set; }

    /// <summary>
    /// 父评论ID
    /// </summary>
    public long ParentCommentId { get; set; }

    /// <summary>
    /// 评论内容
    /// </summary>
    public string? CommentContent { get; set; }

    /// <summary>
    /// 附图地址
    /// </summary>
    public string? ImageAddress { get; set; }

    /// <summary>
    /// 评论时间
    /// </summary>
    public DateTime CommentTime { get; set; }

    /// <summary>
    /// 点赞数
    /// </summary>
    public int Likes { get; set; }

    /// <summary>
    /// 分享数
    /// </summary>
    public int Shares { get; set; }

    /// <summary>
    /// 评论数
    /// </summary>
    public int Comments { get; set; }

    /// <summary>
    /// 发布来源
    /// </summary>
    public short SourceType { get; set; }

    /// <summary>
    /// 评论状态（0-待审核，1-已审核，2-已删除）
    /// </summary>
    public short CommentStatus { get; set; }

    /// <summary>
    /// 评论热度，默认0
    /// </summary>
    public decimal? Hot { get; set; }

    /// <summary>
    /// 是否针对文章评论
    /// </summary>
    public bool IsCommentArticle { get; set; }

    /// <summary>
    /// 评论时间戳
    /// </summary>
    public long? CommentTimespan { get; set; }

    /// <summary>
    /// 举报数
    /// </summary>
    public int Reports { get; set; }

    /// <summary>
    /// 是否展示默认1
    /// </summary>
    public bool IsShow { get; set; }
}
