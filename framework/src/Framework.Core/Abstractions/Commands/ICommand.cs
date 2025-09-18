namespace Framework.Core.Abstractions.Commands;

/// <summary>
/// 命令接口 - 命令模式
/// 提供命令的抽象
/// </summary>
public interface ICommand
{
    /// <summary>
    /// 命令标识
    /// </summary>
    string Id { get; }

    /// <summary>
    /// 命令时间戳
    /// </summary>
    DateTime Timestamp { get; }

    /// <summary>
    /// 命令元数据
    /// </summary>
    IDictionary<string, object> Metadata { get; }
}

/// <summary>
/// 带结果的命令接口 - 命令模式
/// </summary>
/// <typeparam name="TResult">结果类型</typeparam>
public interface ICommand<out TResult> : ICommand
{
}
