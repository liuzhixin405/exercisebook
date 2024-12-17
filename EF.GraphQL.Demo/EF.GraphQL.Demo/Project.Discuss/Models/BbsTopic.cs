using System;
using System.Collections.Generic;

namespace Project.Discuss.Models;

/// <summary>
/// 话题
/// </summary>
public partial class BbsTopic
{
    /// <summary>
    /// 话题ID
    /// </summary>
    public long TopicId { get; set; }

    /// <summary>
    /// 话题标题
    /// </summary>
    public string TopicTitle { get; set; } = null!;

    /// <summary>
    /// 话题内容
    /// </summary>
    public string TopicContent { get; set; } = null!;

    /// <summary>
    /// 附图地址,多个图片用【$$$$】分开
    /// </summary>
    public string? ImageAddress { get; set; }

    /// <summary>
    /// 话题状态（1-激活，2-删除，0-待激活）
    /// </summary>
    public short TopicStatus { get; set; }

    /// <summary>
    /// 是否置顶（0-否，1-是）
    /// </summary>
    public ulong IsTop { get; set; }

    /// <summary>
    /// 话题创建者
    /// </summary>
    public long CreaterUserId { get; set; }

    /// <summary>
    /// 点赞数
    /// </summary>
    public int Likes { get; set; }

    /// <summary>
    /// 评论数
    /// </summary>
    public int Comments { get; set; }

    /// <summary>
    /// 参与人数
    /// </summary>
    public int Participants { get; set; }

    /// <summary>
    /// 话题热度
    /// </summary>
    public decimal Hot { get; set; }

    /// <summary>
    /// 浏览量  
    /// </summary>
    public long? View { get; set; }

    /// <summary>
    /// 排序（默认999999）
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public long CreateTime { get; set; }

    /// <summary>
    /// 更新时间 
    /// </summary>
    public DateTime? UpdateTime { get; set; }
}
