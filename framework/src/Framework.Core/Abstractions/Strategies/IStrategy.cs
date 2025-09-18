namespace Framework.Core.Abstractions.Strategies;

/// <summary>
/// 策略接口 - 策略模式
/// 提供策略的抽象
/// </summary>
public interface IStrategy
{
    /// <summary>
    /// 策略名称
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 策略标识
    /// </summary>
    string Id { get; }

    /// <summary>
    /// 策略优先级（数字越小优先级越高）
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// 执行策略
    /// </summary>
    /// <param name="parameters">参数</param>
    /// <returns>执行结果</returns>
    Task<object?> ExecuteAsync(params object[] parameters);

    /// <summary>
    /// 是否支持指定的参数
    /// </summary>
    /// <param name="parameters">参数</param>
    /// <returns>是否支持</returns>
    bool CanExecute(params object[] parameters);
}

/// <summary>
/// 带结果的策略接口 - 策略模式
/// </summary>
/// <typeparam name="TResult">结果类型</typeparam>
public interface IStrategy<out TResult> : IStrategy
{
    /// <summary>
    /// 执行策略并返回结果
    /// </summary>
    /// <param name="parameters">参数</param>
    /// <returns>执行结果</returns>
    new Task<TResult> ExecuteAsync(params object[] parameters);
}
