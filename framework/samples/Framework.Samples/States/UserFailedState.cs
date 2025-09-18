using Framework.Core.Abstractions.States;

namespace Framework.Samples.States;

/// <summary>
/// 用户失败状态
/// </summary>
public class UserFailedState : IState
{
    /// <inheritdoc />
    public string Name => "UserFailed";

    /// <inheritdoc />
    public string Id => "user-failed";

    /// <inheritdoc />
    public async Task OnEnterAsync(IStateContext context)
    {
        Console.WriteLine("进入用户失败状态");
        context.SetData("FailureTime", DateTime.UtcNow);
        await Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task OnExitAsync(IStateContext context)
    {
        Console.WriteLine("退出用户失败状态");
        await Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task OnProcessAsync(IStateContext context)
    {
        Console.WriteLine("处理用户失败状态");
        await Task.CompletedTask;
    }

    /// <inheritdoc />
    public bool CanTransitionTo(IState targetState)
    {
        // 可以从失败状态转换回注册状态
        return targetState is UserRegistrationState;
    }
}
