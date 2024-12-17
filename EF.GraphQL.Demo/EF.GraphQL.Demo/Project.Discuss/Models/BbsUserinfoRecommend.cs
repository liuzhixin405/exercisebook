using System;
using System.Collections.Generic;

namespace Project.Discuss.Models;

/// <summary>
/// 推荐用户列表
/// </summary>
public partial class BbsUserinfoRecommend
{
    /// <summary>
    /// 推荐ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 推荐用户ID
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime? CreateTime { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int? Sort { get; set; }
}
