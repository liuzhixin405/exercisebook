namespace Framework.Samples.Events;

/// <summary>
/// 用户创建事件
/// </summary>
public class UserCreatedEvent
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 邮箱
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 事件时间
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
