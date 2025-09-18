using Framework.Core.Abstractions.Strategies;
using System.Collections.Concurrent;

namespace Framework.Infrastructure.Strategies;

/// <summary>
/// 策略上下文实现 - 策略模式
/// 提供策略执行的实现
/// </summary>
public class StrategyContext : IStrategyContext
{
    private readonly ConcurrentDictionary<Type, IStrategy> _strategies;

    /// <summary>
    /// 构造函数
    /// </summary>
    public StrategyContext()
    {
        _strategies = new ConcurrentDictionary<Type, IStrategy>();
    }

    /// <inheritdoc />
    public async Task<object?> ExecuteStrategyAsync<TStrategy>(params object[] parameters) 
        where TStrategy : class, IStrategy
    {
        var strategyType = typeof(TStrategy);
        if (!_strategies.TryGetValue(strategyType, out var strategy))
        {
            throw new InvalidOperationException($"Strategy of type {strategyType.Name} is not registered.");
        }

        if (strategy.CanExecute(parameters))
        {
            return await strategy.ExecuteAsync(parameters);
        }

        throw new InvalidOperationException($"Strategy {strategyType.Name} cannot execute with the provided parameters.");
    }

    /// <inheritdoc />
    public async Task<TResult> ExecuteStrategyAsync<TStrategy, TResult>(params object[] parameters) 
        where TStrategy : class, IStrategy<TResult>
    {
        var strategyType = typeof(TStrategy);
        if (!_strategies.TryGetValue(strategyType, out var strategy))
        {
            throw new InvalidOperationException($"Strategy of type {strategyType.Name} is not registered.");
        }

        if (strategy is IStrategy<TResult> typedStrategy)
        {
            if (typedStrategy.CanExecute(parameters))
            {
                return await typedStrategy.ExecuteAsync(parameters);
            }
        }

        throw new InvalidOperationException($"Strategy {strategyType.Name} cannot execute with the provided parameters or does not support result type {typeof(TResult).Name}.");
    }

    /// <inheritdoc />
    public IStrategyContext RegisterStrategy<TStrategy>(TStrategy strategy) 
        where TStrategy : class, IStrategy
    {
        if (strategy == null)
            throw new ArgumentNullException(nameof(strategy));

        var strategyType = typeof(TStrategy);
        _strategies.AddOrUpdate(strategyType, strategy, (key, existing) => strategy);
        return this;
    }

    /// <inheritdoc />
    public IStrategyContext RegisterStrategy<TStrategy>(Func<TStrategy> factory) 
        where TStrategy : class, IStrategy
    {
        if (factory == null)
            throw new ArgumentNullException(nameof(factory));

        var strategy = factory();
        return RegisterStrategy(strategy);
    }

    /// <inheritdoc />
    public TStrategy? GetStrategy<TStrategy>() where TStrategy : class, IStrategy
    {
        var strategyType = typeof(TStrategy);
        if (_strategies.TryGetValue(strategyType, out var strategy))
        {
            return strategy as TStrategy;
        }
        return null;
    }

    /// <inheritdoc />
    public IStrategyContext RemoveStrategy<TStrategy>() where TStrategy : class, IStrategy
    {
        var strategyType = typeof(TStrategy);
        _strategies.TryRemove(strategyType, out _);
        return this;
    }

    /// <inheritdoc />
    public IStrategyContext Clear()
    {
        _strategies.Clear();
        return this;
    }
}
