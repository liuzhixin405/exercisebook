using Framework.Core.Abstractions.Commands;
using Framework.Samples.Services;

namespace Framework.Samples.Commands;

/// <summary>
/// 更新用户命令处理器
/// </summary>
public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand>
{
    private readonly IUserService _userService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="userService">用户服务</param>
    public UpdateUserCommandHandler(IUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    /// <inheritdoc />
    public string Name => "UpdateUserCommandHandler";

    /// <inheritdoc />
    public int Priority => 100;

    /// <inheritdoc />
    public async Task HandleAsync(UpdateUserCommand command)
    {
        Console.WriteLine($"处理更新用户命令: {command.UserId}");
        
        var success = await _userService.UpdateUserAsync(command.UserId, command.UserName, command.Email);
        
        if (success)
        {
            Console.WriteLine($"用户更新成功: {command.UserId}");
        }
        else
        {
            Console.WriteLine($"用户更新失败: {command.UserId}");
        }
    }

    /// <inheritdoc />
    public bool ShouldHandle(UpdateUserCommand command)
    {
        return command.UserId != Guid.Empty && 
               (!string.IsNullOrEmpty(command.UserName) || !string.IsNullOrEmpty(command.Email));
    }
}
