using Framework.Core.Abstractions.States;
using System.Collections.Concurrent;

namespace Framework.Infrastructure.States;

/// <summary>
/// 状态管理器实现 - 状态模式
/// 提供状态管理的实现
/// </summary>
public class StateManager : IStateManager
{
    private readonly ConcurrentDictionary<Type, IState> _currentStates;
    private readonly ConcurrentDictionary<TypePair, List<IStateTransitionHandler>> _transitionHandlers;
    private readonly List<IState> _stateHistory;
    private readonly object _lockObject = new object();

    /// <summary>
    /// 构造函数
    /// </summary>
    public StateManager()
    {
        _currentStates = new ConcurrentDictionary<Type, IState>();
        _transitionHandlers = new ConcurrentDictionary<TypePair, List<IStateTransitionHandler>>();
        _stateHistory = new List<IState>();
    }

    /// <inheritdoc />
    public TState? GetCurrentState<TState>() where TState : class, IState
    {
        var stateType = typeof(TState);
        if (_currentStates.TryGetValue(stateType, out var state))
        {
            return state as TState;
        }
        return null;
    }

    /// <inheritdoc />
    public void SetState<TState>(TState state) where TState : class, IState
    {
        if (state == null)
            throw new ArgumentNullException(nameof(state));

        var stateType = typeof(TState);
        var previousState = _currentStates.GetValueOrDefault(stateType);

        _currentStates.AddOrUpdate(stateType, state, (key, existing) => state);

        // 添加到历史记录
        lock (_lockObject)
        {
            _stateHistory.Add(state);
        }

        // 触发状态变化事件
        OnStateChanged(previousState, state);
    }

    /// <inheritdoc />
    public async Task<bool> TransitionToAsync<TState>(TState state) where TState : class, IState
    {
        if (state == null)
            throw new ArgumentNullException(nameof(state));

        var stateType = typeof(TState);
        var currentState = _currentStates.GetValueOrDefault(stateType);

        // 检查是否可以转换
        if (currentState != null && !currentState.CanTransitionTo(state))
        {
            return false;
        }

        // 执行转换处理器
        var transitionHandlers = GetTransitionHandlers(currentState?.GetType(), stateType);
        foreach (var handler in transitionHandlers)
        {
            if (!await handler.HandleTransitionAsync(currentState, state, CreateStateContext()))
            {
                return false;
            }
        }

        // 执行状态转换
        var context = CreateStateContext();
        
        if (currentState != null)
        {
            await currentState.OnExitAsync(context);
        }

        await state.OnEnterAsync(context);
        SetState(state);

        return true;
    }

    /// <inheritdoc />
    public void RegisterTransitionHandler<TFromState, TToState>(IStateTransitionHandler<TFromState, TToState> handler)
        where TFromState : class, IState
        where TToState : class, IState
    {
        if (handler == null)
            throw new ArgumentNullException(nameof(handler));

        var key = new TypePair(typeof(TFromState), typeof(TToState));
        var wrapper = new StateTransitionHandlerWrapper<TFromState, TToState>(handler);

        _transitionHandlers.AddOrUpdate(
            key,
            new List<IStateTransitionHandler> { wrapper },
            (k, existing) =>
            {
                existing.Add(wrapper);
                return existing;
            });
    }

    /// <inheritdoc />
    public IEnumerable<IState> GetStateHistory()
    {
        lock (_lockObject)
        {
            return _stateHistory.ToList();
        }
    }

    /// <inheritdoc />
    public void ClearHistory()
    {
        lock (_lockObject)
        {
            _stateHistory.Clear();
        }
    }

    /// <inheritdoc />
    public event EventHandler<StateChangedEventArgs>? StateChanged;

    /// <summary>
    /// 获取转换处理器
    /// </summary>
    /// <param name="fromStateType">源状态类型</param>
    /// <param name="toStateType">目标状态类型</param>
    /// <returns>转换处理器列表</returns>
    private IEnumerable<IStateTransitionHandler> GetTransitionHandlers(Type? fromStateType, Type toStateType)
    {
        var key = new TypePair(fromStateType, toStateType);
        if (_transitionHandlers.TryGetValue(key, out var handlers))
        {
            return handlers.OrderBy(h => h.Priority);
        }
        return Enumerable.Empty<IStateTransitionHandler>();
    }

    /// <summary>
    /// 创建状态上下文
    /// </summary>
    /// <returns>状态上下文</returns>
    private IStateContext CreateStateContext()
    {
        return new StateContext();
    }

    /// <summary>
    /// 触发状态变化事件
    /// </summary>
    /// <param name="fromState">源状态</param>
    /// <param name="toState">目标状态</param>
    private void OnStateChanged(IState? fromState, IState toState)
    {
        var context = CreateStateContext();
        var args = new StateChangedEventArgs(fromState, toState, context);
        StateChanged?.Invoke(this, args);
    }
}

/// <summary>
/// 状态上下文实现
/// </summary>
public class StateContext : IStateContext
{
    private readonly Dictionary<string, object> _data;

    /// <summary>
    /// 构造函数
    /// </summary>
    public StateContext()
    {
        _data = new Dictionary<string, object>();
    }

    /// <inheritdoc />
    public IDictionary<string, object> Data => _data;

    /// <inheritdoc />
    public T? GetData<T>(string key)
    {
        if (_data.TryGetValue(key, out var value) && value is T typedValue)
        {
            return typedValue;
        }
        return default;
    }

    /// <inheritdoc />
    public void SetData<T>(string key, T value)
    {
        _data[key] = value!;
    }

    /// <inheritdoc />
    public bool RemoveData(string key)
    {
        return _data.Remove(key);
    }

    /// <inheritdoc />
    public void ClearData()
    {
        _data.Clear();
    }
}

/// <summary>
/// 类型对
/// </summary>
internal record TypePair(Type? From, Type To);

/// <summary>
/// 非泛型内部状态转换处理器接口（用于内部存储）
/// </summary>
internal interface IStateTransitionHandler
{
    string Name { get; }
    int Priority { get; }
    Task<bool> HandleTransitionAsync(IState? fromState, IState toState, IStateContext context);
}

/// <summary>
/// 状态转换处理器包装器
/// </summary>
/// <typeparam name="TFromState">源状态类型</typeparam>
/// <typeparam name="TToState">目标状态类型</typeparam>
internal class StateTransitionHandlerWrapper<TFromState, TToState> : IStateTransitionHandler
    where TFromState : class, IState
    where TToState : class, IState
{
    private readonly Framework.Core.Abstractions.States.IStateTransitionHandler<TFromState, TToState> _handler;

    public StateTransitionHandlerWrapper(Framework.Core.Abstractions.States.IStateTransitionHandler<TFromState, TToState> handler)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
    }

    public string Name => _handler.Name;

    public int Priority => _handler.Priority;

    public async Task<bool> HandleTransitionAsync(IState? fromState, IState toState, IStateContext context)
    {
        if (fromState is TFromState typedFromState && toState is TToState typedToState)
        {
            if (_handler.ShouldHandle(typedFromState, typedToState))
            {
                return await _handler.HandleTransitionAsync(typedFromState, typedToState, context);
            }
        }
        return true;
    }
}
