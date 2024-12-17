using System;
using System.Collections.Generic;

namespace Project.Discuss.Models;

public partial class BbsUserNotification
{
    /// <summary>
    /// 通知id
    /// </summary>
    public long NotificationId { get; set; }

    /// <summary>
    /// 接收通知的用户ID
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// 通知类型（ 0-全部 1-点赞 2-评论 3-关注 4-提及 5-回复 6-收藏  )
    /// </summary>
    public sbyte Type { get; set; }

    /// <summary>
    /// 发送通知的用户ID（点赞者、评论者、回复者等）
    /// </summary>
    public long SenderId { get; set; }

    /// <summary>
    /// 通知的内容（例如“用户A评论了你的文章”）
    /// </summary>
    public string Content { get; set; } = null!;

    /// <summary>
    /// 跳转链接，指向具体的资源，如文章、评论、用户主页等
    /// </summary>
    public string? Link { get; set; }

    /// <summary>
    /// 是否已读（1-已读，0-未读）
    /// </summary>
    public sbyte? IsRead { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime? CreatedTime { get; set; }

    /// <summary>
    /// 更新时间，更新为已读时使用
    /// </summary>
    public DateTime? UpdatedTime { get; set; }

    /// <summary>
    /// 内容类型1:文章 2:评论 3:话题 4:用户
    /// </summary>
    public sbyte? Category { get; set; }

    /// <summary>
    /// 创建时间戳
    /// </summary>
    public long? CreatedTimespan { get; set; }

    /// <summary>
    /// 修改时间戳
    /// </summary>
    public long? UpdatedTimespan { get; set; }

    public virtual BbsUserinfo Sender { get; set; } = null!;

    public virtual BbsUserinfo User { get; set; } = null!;
}
