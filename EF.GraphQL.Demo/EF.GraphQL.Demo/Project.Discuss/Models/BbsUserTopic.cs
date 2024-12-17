using System;
using System.Collections.Generic;

namespace Project.Discuss.Models;

/// <summary>
/// 用户参与的话题
/// </summary>
public partial class BbsUserTopic
{
    /// <summary>
    /// 话题ID
    /// </summary>
    public long TopicId { get; set; }

    /// <summary>
    /// 用户ID
    /// </summary>
    public long UserId { get; set; }
}
