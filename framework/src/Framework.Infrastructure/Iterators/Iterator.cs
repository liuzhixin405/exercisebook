namespace Framework.Infrastructure.Iterators;

/// <summary>
/// 迭代器实现 - 迭代器模式
/// </summary>
/// <typeparam name="T">元素类型</typeparam>
public class Iterator<T> : IIterator<T>
{
    private readonly T[] _items;
    private int _currentIndex;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="items">元素数组</param>
    public Iterator(T[] items)
    {
        _items = items ?? throw new ArgumentNullException(nameof(items));
        _currentIndex = -1;
    }

    /// <inheritdoc />
    public T Current
    {
        get
        {
            if (_currentIndex < 0 || _currentIndex >= _items.Length)
            {
                throw new InvalidOperationException("Iterator is not positioned on a valid element.");
            }
            return _items[_currentIndex];
        }
    }

    /// <inheritdoc />
    public bool HasNext => _currentIndex + 1 < _items.Length;

    /// <inheritdoc />
    public bool MoveNext()
    {
        if (HasNext)
        {
            _currentIndex++;
            return true;
        }
        return false;
    }

    /// <inheritdoc />
    public void Reset()
    {
        _currentIndex = -1;
    }
}

/// <summary>
/// 聚合实现 - 迭代器模式
/// </summary>
/// <typeparam name="T">元素类型</typeparam>
public class Aggregate<T> : IAggregate<T>
{
    private readonly List<T> _items;

    /// <summary>
    /// 构造函数
    /// </summary>
    public Aggregate()
    {
        _items = new List<T>();
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="items">初始元素</param>
    public Aggregate(IEnumerable<T> items)
    {
        _items = new List<T>(items ?? throw new ArgumentNullException(nameof(items)));
    }

    /// <inheritdoc />
    public void Add(T item)
    {
        _items.Add(item);
    }

    /// <inheritdoc />
    public bool Remove(T item)
    {
        return _items.Remove(item);
    }

    /// <inheritdoc />
    public int Count => _items.Count;

    /// <inheritdoc />
    public void Clear()
    {
        _items.Clear();
    }

    /// <inheritdoc />
    public IIterator<T> GetIterator()
    {
        return new Iterator<T>(_items.ToArray());
    }
}
