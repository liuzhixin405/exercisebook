using Framework.Core.Abstractions.States;

namespace Framework.Samples.States;

/// <summary>
/// 用户激活状态
/// </summary>
public class UserActiveState : IState
{
    /// <inheritdoc />
    public string Name => "UserActive";

    /// <inheritdoc />
    public string Id => "user-active";

    /// <inheritdoc />
    public async Task OnEnterAsync(IStateContext context)
    {
        Console.WriteLine("进入用户激活状态");
        context.SetData("ActivationTime", DateTime.UtcNow);
        await Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task OnExitAsync(IStateContext context)
    {
        Console.WriteLine("退出用户激活状态");
        await Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task OnProcessAsync(IStateContext context)
    {
        Console.WriteLine("处理用户激活状态");
        await Task.CompletedTask;
    }

    /// <inheritdoc />
    public bool CanTransitionTo(IState targetState)
    {
        // 可以从激活状态转换到暂停状态或删除状态
        return targetState is UserSuspendedState || targetState is UserDeletedState;
    }
}
