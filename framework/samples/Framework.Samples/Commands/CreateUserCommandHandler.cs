using Framework.Core.Abstractions.Commands;
using Framework.Samples.Services;

namespace Framework.Samples.Commands;

/// <summary>
/// 创建用户命令处理器
/// </summary>
public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand>
{
    private readonly IUserService _userService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="userService">用户服务</param>
    public CreateUserCommandHandler(IUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    /// <inheritdoc />
    public string Name => "CreateUserCommandHandler";

    /// <inheritdoc />
    public int Priority => 100;

    /// <inheritdoc />
    public async Task HandleAsync(CreateUserCommand command)
    {
        Console.WriteLine($"处理创建用户命令: {command.UserName} ({command.Email})");
        
        var userId = await _userService.CreateUserAsync(command.UserName, command.Email, command.Password);
        
        Console.WriteLine($"用户创建成功，ID: {userId}");
    }

    /// <inheritdoc />
    public bool ShouldHandle(CreateUserCommand command)
    {
        return !string.IsNullOrEmpty(command.UserName) && 
               !string.IsNullOrEmpty(command.Email) && 
               !string.IsNullOrEmpty(command.Password);
    }
}
