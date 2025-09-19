using Framework.Core.Abstractions.States;

namespace Framework.Samples.States;

/// <summary>
/// 用户删除状态
/// </summary>
public class UserDeletedState : IState
{
    /// <inheritdoc />
    public string Name => "UserDeleted";

    /// <inheritdoc />
    public string Id => "user-deleted";

    /// <inheritdoc />
    public async Task OnEnterAsync(IStateContext context)
    {
        Console.WriteLine("进入用户删除状态");
        context.SetData("DeletionTime", DateTime.UtcNow);
        await Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task OnExitAsync(IStateContext context)
    {
        Console.WriteLine("退出用户删除状态");
        await Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task OnProcessAsync(IStateContext context)
    {
        Console.WriteLine("处理用户删除状态");
        await Task.CompletedTask;
    }

    /// <inheritdoc />
    public bool CanTransitionTo(IState targetState)
    {
        // 删除状态是终态，不能转换到其他状态
        return false;
    }
}