using Framework.Core.Abstractions.Events;

namespace Framework.Samples.Events;

/// <summary>
/// 用户更新事件处理器
/// </summary>
public class UserUpdatedEventHandler : IEventHandler<UserUpdatedEvent>
{
    /// <inheritdoc />
    public string Name => "UserUpdatedEventHandler";

    /// <inheritdoc />
    public int Priority => 100;

    /// <inheritdoc />
    public async Task HandleAsync(UserUpdatedEvent @event)
    {
        Console.WriteLine($"处理用户更新事件: {@event.UserId}");
        Console.WriteLine($"用户名: {@event.OldName} -> {@event.NewName}");
        Console.WriteLine($"邮箱: {@event.OldEmail} -> {@event.NewEmail}");
        
        // 模拟处理时间
        await Task.Delay(50);
        
        Console.WriteLine("用户更新事件处理完成");
    }

    /// <inheritdoc />
    public bool ShouldHandle(UserUpdatedEvent @event)
    {
        return @event.UserId != Guid.Empty;
    }
}
