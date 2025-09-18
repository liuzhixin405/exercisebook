using System.Collections.Concurrent;
using System.Text.Json;

namespace Framework.Infrastructure.Memento;

/// <summary>
/// 备忘录管理器实现 - 备忘录模式
/// </summary>
public class MementoManager : IMementoManager
{
    private readonly ConcurrentDictionary<string, IMemento> _mementos;

    /// <summary>
    /// 构造函数
    /// </summary>
    public MementoManager()
    {
        _mementos = new ConcurrentDictionary<string, IMemento>();
    }

    /// <inheritdoc />
    public IMemento SaveState<T>(T state)
    {
        if (state == null)
            throw new ArgumentNullException(nameof(state));

        var memento = new Memento<T>(state);
        _mementos.AddOrUpdate(memento.Id, memento, (key, existing) => memento);
        return memento;
    }

    /// <inheritdoc />
    public T RestoreState<T>(IMemento memento)
    {
        if (memento == null)
            throw new ArgumentNullException(nameof(memento));

        return memento.GetState<T>();
    }

    /// <inheritdoc />
    public IMemento? GetMemento(string id)
    {
        if (string.IsNullOrEmpty(id))
            return null;

        _mementos.TryGetValue(id, out var memento);
        return memento;
    }

    /// <inheritdoc />
    public IEnumerable<IMemento> GetAllMementos()
    {
        return _mementos.Values.OrderBy(m => m.CreatedAt);
    }

    /// <inheritdoc />
    public bool RemoveMemento(string id)
    {
        if (string.IsNullOrEmpty(id))
            return false;

        return _mementos.TryRemove(id, out _);
    }

    /// <inheritdoc />
    public void Clear()
    {
        _mementos.Clear();
    }
}

/// <summary>
/// 备忘录实现 - 备忘录模式
/// </summary>
/// <typeparam name="T">状态类型</typeparam>
public class Memento<T> : IMemento
{
    private readonly T _state;
    private readonly string _serializedState;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="state">状态</param>
    public Memento(T state)
    {
        _state = state ?? throw new ArgumentNullException(nameof(state));
        Id = Guid.NewGuid().ToString();
        CreatedAt = DateTime.UtcNow;
        _serializedState = JsonSerializer.Serialize(state);
    }

    /// <inheritdoc />
    public string Id { get; }

    /// <inheritdoc />
    public DateTime CreatedAt { get; }

    /// <inheritdoc />
    public object State => _state;

    /// <inheritdoc />
    public TState GetState<TState>()
    {
        if (typeof(TState) == typeof(T))
        {
            return (TState)(object)_state;
        }

        // 尝试从序列化数据反序列化
        return JsonSerializer.Deserialize<TState>(_serializedState) 
            ?? throw new InvalidOperationException($"Cannot deserialize state to type {typeof(TState).Name}");
    }

    /// <inheritdoc />
    public void SetState<TState>(TState state)
    {
        throw new InvalidOperationException("Memento state is immutable. Create a new memento instead.");
    }
}
