namespace Framework.Core.Abstractions.Strategies;

/// <summary>
/// 策略上下文接口 - 策略模式
/// 提供策略执行的抽象
/// </summary>
public interface IStrategyContext
{
    /// <summary>
    /// 执行策略
    /// </summary>
    /// <typeparam name="TStrategy">策略类型</typeparam>
    /// <param name="parameters">参数</param>
    /// <returns>执行结果</returns>
    Task<object?> ExecuteStrategyAsync<TStrategy>(params object[] parameters) 
        where TStrategy : class, IStrategy;

    /// <summary>
    /// 执行策略（带结果类型）
    /// </summary>
    /// <typeparam name="TStrategy">策略类型</typeparam>
    /// <typeparam name="TResult">结果类型</typeparam>
    /// <param name="parameters">参数</param>
    /// <returns>执行结果</returns>
    Task<TResult> ExecuteStrategyAsync<TStrategy, TResult>(params object[] parameters) 
        where TStrategy : class, IStrategy<TResult>;

    /// <summary>
    /// 注册策略
    /// </summary>
    /// <typeparam name="TStrategy">策略类型</typeparam>
    /// <param name="strategy">策略实例</param>
    /// <returns>策略上下文</returns>
    IStrategyContext RegisterStrategy<TStrategy>(TStrategy strategy) 
        where TStrategy : class, IStrategy;

    /// <summary>
    /// 注册策略（工厂）
    /// </summary>
    /// <typeparam name="TStrategy">策略类型</typeparam>
    /// <param name="factory">策略工厂</param>
    /// <returns>策略上下文</returns>
    IStrategyContext RegisterStrategy<TStrategy>(Func<TStrategy> factory) 
        where TStrategy : class, IStrategy;

    /// <summary>
    /// 获取策略
    /// </summary>
    /// <typeparam name="TStrategy">策略类型</typeparam>
    /// <returns>策略实例</returns>
    TStrategy? GetStrategy<TStrategy>() where TStrategy : class, IStrategy;

    /// <summary>
    /// 移除策略
    /// </summary>
    /// <typeparam name="TStrategy">策略类型</typeparam>
    /// <returns>策略上下文</returns>
    IStrategyContext RemoveStrategy<TStrategy>() where TStrategy : class, IStrategy;

    /// <summary>
    /// 清空所有策略
    /// </summary>
    /// <returns>策略上下文</returns>
    IStrategyContext Clear();
}
