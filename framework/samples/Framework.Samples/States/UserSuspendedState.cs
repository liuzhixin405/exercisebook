using Framework.Core.Abstractions.States;

namespace Framework.Samples.States;

/// <summary>
/// 用户暂停状态
/// </summary>
public class UserSuspendedState : IState
{
    /// <inheritdoc />
    public string Name => "UserSuspended";

    /// <inheritdoc />
    public string Id => "user-suspended";

    /// <inheritdoc />
    public async Task OnEnterAsync(IStateContext context)
    {
        Console.WriteLine("进入用户暂停状态");
        context.SetData("SuspensionTime", DateTime.UtcNow);
        await Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task OnExitAsync(IStateContext context)
    {
        Console.WriteLine("退出用户暂停状态");
        await Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task OnProcessAsync(IStateContext context)
    {
        Console.WriteLine("处理用户暂停状态");
        await Task.CompletedTask;
    }

    /// <inheritdoc />
    public bool CanTransitionTo(IState targetState)
    {
        // 可以从暂停状态转换到激活状态或删除状态
        return targetState is UserActiveState || targetState is UserDeletedState;
    }
}