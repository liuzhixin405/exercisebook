using Framework.Core.Abstractions.States;

namespace Framework.Samples.States;

/// <summary>
/// 用户注册状态
/// </summary>
public class UserRegistrationState : IState
{
    /// <inheritdoc />
    public string Name => "UserRegistration";

    /// <inheritdoc />
    public string Id => "user-registration";

    /// <inheritdoc />
    public async Task OnEnterAsync(IStateContext context)
    {
        Console.WriteLine("进入用户注册状态");
        context.SetData("RegistrationStartTime", DateTime.UtcNow);
        await Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task OnExitAsync(IStateContext context)
    {
        Console.WriteLine("退出用户注册状态");
        var startTime = context.GetData<DateTime>("RegistrationStartTime");
        var duration = DateTime.UtcNow - startTime;
        Console.WriteLine($"注册耗时: {duration.TotalMilliseconds}ms");
        await Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task OnProcessAsync(IStateContext context)
    {
        Console.WriteLine("处理用户注册状态");
        await Task.CompletedTask;
    }

    /// <inheritdoc />
    public bool CanTransitionTo(IState targetState)
    {
        // 可以从注册状态转换到激活状态或失败状态
        return targetState is UserActiveState || targetState is UserFailedState;
    }
}
