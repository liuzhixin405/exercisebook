namespace Framework.Samples.Events;

/// <summary>
/// 用户更新事件
/// </summary>
public class UserUpdatedEvent
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// 旧用户名
    /// </summary>
    public string OldName { get; set; } = string.Empty;

    /// <summary>
    /// 新用户名
    /// </summary>
    public string NewName { get; set; } = string.Empty;

    /// <summary>
    /// 旧邮箱
    /// </summary>
    public string OldEmail { get; set; } = string.Empty;

    /// <summary>
    /// 新邮箱
    /// </summary>
    public string NewEmail { get; set; } = string.Empty;

    /// <summary>
    /// 事件时间
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
