using System;
using System.Collections.Generic;

namespace Project.Discuss.Models;

/// <summary>
/// 一周明星用户表
/// </summary>
public partial class BbsRakingUserinfo
{
    /// <summary>
    /// 用户id
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// 排名,默认排名999999
    /// </summary>
    public int RankNo { get; set; }

    /// <summary>
    /// 用户昵称
    /// </summary>
    public string UserName { get; set; } = null!;

    /// <summary>
    /// 上周平均点赞数
    /// </summary>
    public int AvgLike { get; set; }

    /// <summary>
    /// 上周观点数
    /// </summary>
    public int ArticleCount { get; set; }

    /// <summary>
    /// 人工插入排名,默认排名999999
    /// </summary>
    public int CustomRankNo { get; set; }

    /// <summary>
    /// 人工插入排名过期时间
    /// </summary>
    public DateTime CustomRankNoExpireTime { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime ModifyTime { get; set; }

    /// <summary>
    /// 粉丝数
    /// </summary>
    public int Followcount { get; set; }
}
