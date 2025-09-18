namespace Framework.Infrastructure.Memento;

/// <summary>
/// 备忘录接口 - 备忘录模式
/// 提供状态保存的抽象
/// </summary>
public interface IMemento
{
    /// <summary>
    /// 备忘录标识
    /// </summary>
    string Id { get; }

    /// <summary>
    /// 创建时间
    /// </summary>
    DateTime CreatedAt { get; }

    /// <summary>
    /// 状态数据
    /// </summary>
    object State { get; }

    /// <summary>
    /// 获取状态
    /// </summary>
    /// <typeparam name="T">状态类型</typeparam>
    /// <returns>状态</returns>
    T GetState<T>();

    /// <summary>
    /// 设置状态
    /// </summary>
    /// <typeparam name="T">状态类型</typeparam>
    /// <param name="state">状态</param>
    void SetState<T>(T state);
}

/// <summary>
/// 备忘录管理器接口 - 备忘录模式
/// </summary>
public interface IMementoManager
{
    /// <summary>
    /// 保存状态
    /// </summary>
    /// <typeparam name="T">状态类型</typeparam>
    /// <param name="state">状态</param>
    /// <returns>备忘录</returns>
    IMemento SaveState<T>(T state);

    /// <summary>
    /// 恢复状态
    /// </summary>
    /// <typeparam name="T">状态类型</typeparam>
    /// <param name="memento">备忘录</param>
    /// <returns>状态</returns>
    T RestoreState<T>(IMemento memento);

    /// <summary>
    /// 获取备忘录
    /// </summary>
    /// <param name="id">备忘录标识</param>
    /// <returns>备忘录</returns>
    IMemento? GetMemento(string id);

    /// <summary>
    /// 获取所有备忘录
    /// </summary>
    /// <returns>备忘录列表</returns>
    IEnumerable<IMemento> GetAllMementos();

    /// <summary>
    /// 移除备忘录
    /// </summary>
    /// <param name="id">备忘录标识</param>
    /// <returns>是否移除成功</returns>
    bool RemoveMemento(string id);

    /// <summary>
    /// 清空所有备忘录
    /// </summary>
    void Clear();
}

/// <summary>
/// 可保存状态接口 - 备忘录模式
/// </summary>
public interface ISaveable
{
    /// <summary>
    /// 创建备忘录
    /// </summary>
    /// <returns>备忘录</returns>
    IMemento CreateMemento();

    /// <summary>
    /// 从备忘录恢复状态
    /// </summary>
    /// <param name="memento">备忘录</param>
    void RestoreFromMemento(IMemento memento);
}
