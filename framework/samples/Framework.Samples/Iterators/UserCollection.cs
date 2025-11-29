using Framework.Infrastructure.Iterators;

namespace Framework.Samples.Iterators;

/// <summary>
/// 用户集合 - 迭代器模式示例
/// </summary>
public class UserCollection
{
    private readonly List<UserData> _users = new();

    public void AddUser(UserData user)
    {
        _users.Add(user);
        Console.WriteLine($"[迭代器示例] 添加用户: {user.Name}");
    }

    public IIterator<UserData> GetIterator()
    {
        return new Iterator<UserData>(_users.ToArray());
    }

    public IAggregate<UserData> GetAggregate()
    {
        return new Aggregate<UserData>(_users);
    }
}

/// <summary>
/// 用户数据
/// </summary>
public class UserData
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

/// <summary>
/// 过滤迭代器 - 迭代器模式扩展示例
/// </summary>
public class FilterIterator<T> : IIterator<T>
{
    private readonly IIterator<T> _innerIterator;
    private readonly Func<T, bool> _predicate;
    private T? _current;
    private bool _hasValidCurrent;

    public FilterIterator(IIterator<T> innerIterator, Func<T, bool> predicate)
    {
        _innerIterator = innerIterator ?? throw new ArgumentNullException(nameof(innerIterator));
        _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        _hasValidCurrent = false;
    }

    public T Current
    {
        get
        {
            if (!_hasValidCurrent)
                throw new InvalidOperationException("Iterator is not positioned on a valid element.");
            return _current!;
        }
    }

    public bool HasNext
    {
        get
        {
            // 如果已有有效的当前元素，返回true
            if (_hasValidCurrent)
                return true;

            // 尝试找到下一个符合条件的元素
            while (_innerIterator.HasNext)
            {
                _innerIterator.MoveNext();
                if (_predicate(_innerIterator.Current))
                {
                    return true;
                }
            }
            return false;
        }
    }

    public bool MoveNext()
    {
        while (_innerIterator.MoveNext())
        {
            var current = _innerIterator.Current;
            if (_predicate(current))
            {
                _current = current;
                _hasValidCurrent = true;
                return true;
            }
        }
        _hasValidCurrent = false;
        return false;
    }

    public void Reset()
    {
        _innerIterator.Reset();
        _hasValidCurrent = false;
        _current = default;
    }
}
