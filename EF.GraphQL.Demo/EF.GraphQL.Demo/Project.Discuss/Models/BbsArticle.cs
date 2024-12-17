using System;
using System.Collections.Generic;

namespace Project.Discuss.Models;

/// <summary>
/// 用户发布文章
/// </summary>
public partial class BbsArticle
{
    /// <summary>
    /// 文章ID
    /// </summary>
    public long ArticleId { get; set; }

    /// <summary>
    /// 用户ID
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// 文章标题
    /// </summary>
    public string ArticleTitle { get; set; } = null!;

    /// <summary>
    /// 文章详情
    /// </summary>
    public string ArticleContent { get; set; } = null!;

    /// <summary>
    /// 附图地址,JSON格式存储
    /// </summary>
    public string? ImageAddress { get; set; }

    /// <summary>
    /// 观点类型(空,多,中立)-1：默认，0:空，1:多，2：中立
    /// </summary>
    public short? StandPoint { get; set; }

    /// <summary>
    /// 发布时间
    /// </summary>
    public DateTime PublishTime { get; set; }

    /// <summary>
    /// 文章状态(-1-草稿,0-待审核，1-审核通过，2-下架，3-删除)
    /// </summary>
    public short Status { get; set; }

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
    /// 举报数
    /// </summary>
    public int Reports { get; set; }

    /// <summary>
    /// 排序(likes+comments)
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 发布方式,0:手动，1：自动
    /// </summary>
    public sbyte PublishingMode { get; set; }

    /// <summary>
    /// 发布来源
    /// </summary>
    public short SourceType { get; set; }

    /// <summary>
    /// 回复内容冗余数据（3条，JSON格式保存）
    /// </summary>
    public string? Reply { get; set; }

    /// <summary>
    /// 是否置顶，默认0
    /// </summary>
    public ulong IsTop { get; set; }

    /// <summary>
    /// 置顶结束时间
    /// </summary>
    public long? TopEndTime { get; set; }

    /// <summary>
    /// 文章热度，默认0
    /// </summary>
    public decimal Hot { get; set; }

    /// <summary>
    /// 审核管理ID
    /// </summary>
    public long EditUserId { get; set; }

    /// <summary>
    /// 用户类型(普通用户，媒体，项目方等)，32：非小号官方
    /// </summary>
    public short UserType { get; set; }

    /// <summary>
    /// 发布用户的昵称
    /// </summary>
    public string UserNickname { get; set; } = null!;

    /// <summary>
    /// 禁言处理状态，0：待处理，1：无需处理
    /// </summary>
    public sbyte ForbiddenState { get; set; }

    /// <summary>
    /// 发布时间
    /// </summary>
    public long PublishDateTime { get; set; }

    /// <summary>
    /// 置顶显示位置：0-全部，1-最新，2-热门，3-关注，4-最新+热门
    /// </summary>
    public short TopArea { get; set; }

    /// <summary>
    /// datacenter库中push_tag_subscribe 表的订阅id
    /// </summary>
    public uint SubscribeType { get; set; }

    /// <summary>
    /// 收藏数量
    /// </summary>
    public int CollectionCount { get; set; }

    /// <summary>
    /// 文章类型，0：短文，1：长文
    /// </summary>
    public int Articletype { get; set; }

    /// <summary>
    /// 对应的资讯id
    /// </summary>
    public long NewsId { get; set; }

    /// <summary>
    /// 评论用户数
    /// </summary>
    public int CommentUserCount { get; set; }

    /// <summary>
    /// 置顶开始时间
    /// </summary>
    public long? TopStartTime { get; set; }

    /// <summary>
    /// 浏览量
    /// </summary>
    public int View { get; set; }

    /// <summary>
    /// 浏览时长  单位：秒
    /// </summary>
    public long? ViewDuration { get; set; }

    /// <summary>
    /// 转发量
    /// </summary>
    public int Forwardings { get; set; }

    /// <summary>
    /// 转发父id
    /// </summary>
    public long? ForwardingFid { get; set; }

    /// <summary>
    /// 内容新鲜度(每天更新一次)
    /// </summary>
    public decimal? Freshness { get; set; }

    /// <summary>
    /// 下架理由
    /// </summary>
    public string? ShelfReason { get; set; }

    /// <summary>
    /// 审核时间
    /// </summary>
    public DateTime? AuditTime { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public long? CreatedTime { get; set; }

    /// <summary>
    /// 修改时间
    /// </summary>
    public long? EditTime { get; set; }

    /// <summary>
    /// 币种code
    /// </summary>
    public string? CoinCode { get; set; }

    /// <summary>
    /// 文章内容m5
    /// </summary>
    public string? ContentMd5 { get; set; }

    public virtual ICollection<BbsArticleCoin> BbsArticleCoins { get; set; } = new List<BbsArticleCoin>();
}
