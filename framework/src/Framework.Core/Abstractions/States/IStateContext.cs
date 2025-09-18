namespace Framework.Core.Abstractions.States;

/// <summary>
/// 状态上下文接口 - 状态模式
/// 提供状态上下文的抽象
/// </summary>
public interface IStateContext
{
    /// <summary>
    /// 上下文数据
    /// </summary>
    IDictionary<string, object> Data { get; }

    /// <summary>
    /// 获取数据
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="key">键</param>
    /// <returns>数据值</returns>
    T? GetData<T>(string key);

    /// <summary>
    /// 设置数据
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    void SetData<T>(string key, T value);

    /// <summary>
    /// 移除数据
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>是否移除成功</returns>
    bool RemoveData(string key);

    /// <summary>
    /// 清空数据
    /// </summary>
    void ClearData();
}
