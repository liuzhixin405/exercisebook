namespace Framework.Infrastructure.Iterators;

/// <summary>
/// 迭代器接口 - 迭代器模式
/// 提供集合遍历的抽象
/// </summary>
/// <typeparam name="T">元素类型</typeparam>
public interface IIterator<out T>
{
    /// <summary>
    /// 当前元素
    /// </summary>
    T Current { get; }

    /// <summary>
    /// 是否有下一个元素
    /// </summary>
    bool HasNext { get; }

    /// <summary>
    /// 移动到下一个元素
    /// </summary>
    /// <returns>是否成功移动</returns>
    bool MoveNext();

    /// <summary>
    /// 重置迭代器
    /// </summary>
    void Reset();
}

/// <summary>
/// 可迭代接口 - 迭代器模式
/// </summary>
/// <typeparam name="T">元素类型</typeparam>
public interface IIterable<out T>
{
    /// <summary>
    /// 获取迭代器
    /// </summary>
    /// <returns>迭代器</returns>
    IIterator<T> GetIterator();
}

/// <summary>
/// 聚合接口 - 迭代器模式
/// </summary>
/// <typeparam name="T">元素类型</typeparam>
public interface IAggregate<T> : IIterable<T>
{
    /// <summary>
    /// 添加元素
    /// </summary>
    /// <param name="item">元素</param>
    void Add(T item);

    /// <summary>
    /// 移除元素
    /// </summary>
    /// <param name="item">元素</param>
    /// <returns>是否移除成功</returns>
    bool Remove(T item);

    /// <summary>
    /// 获取元素数量
    /// </summary>
    int Count { get; }

    /// <summary>
    /// 清空所有元素
    /// </summary>
    void Clear();
}
