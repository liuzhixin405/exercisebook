using Framework.Core.Abstractions.Events;
using Framework.Samples.Services;

namespace Framework.Samples.Events;

/// <summary>
/// 用户创建事件处理器
/// </summary>
public class UserCreatedEventHandler : IEventHandler<UserCreatedEvent>
{
    private readonly IEmailService _emailService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="emailService">邮件服务</param>
    public UserCreatedEventHandler(IEmailService emailService)
    {
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
    }

    /// <inheritdoc />
    public string Name => "UserCreatedEventHandler";

    /// <inheritdoc />
    public int Priority => 100;

    /// <inheritdoc />
    public async Task HandleAsync(UserCreatedEvent @event)
    {
        Console.WriteLine($"处理用户创建事件: {@event.UserName} ({@event.Email})");
        
        // 发送欢迎邮件
        await _emailService.SendWelcomeEmailAsync(@event.Email, @event.UserName);
        
        Console.WriteLine("用户创建事件处理完成");
    }

    /// <inheritdoc />
    public bool ShouldHandle(UserCreatedEvent @event)
    {
        return !string.IsNullOrEmpty(@event.Email);
    }
}
