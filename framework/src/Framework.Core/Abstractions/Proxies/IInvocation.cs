using System.Reflection;

namespace Framework.Core.Abstractions.Proxies;

/// <summary>
/// 方法调用信息接口 - 代理模式
/// 提供方法调用信息的抽象
/// </summary>
public interface IInvocation
{
    /// <summary>
    /// 目标对象
    /// </summary>
    object Target { get; }

    /// <summary>
    /// 方法信息
    /// </summary>
    MethodInfo Method { get; }

    /// <summary>
    /// 方法参数
    /// </summary>
    object[] Arguments { get; }

    /// <summary>
    /// 返回值
    /// </summary>
    object? ReturnValue { get; set; }

    /// <summary>
    /// 是否已处理
    /// </summary>
    bool IsHandled { get; set; }

    /// <summary>
    /// 调用上下文
    /// </summary>
    IDictionary<string, object> Context { get; }

    /// <summary>
    /// 继续执行下一个拦截器
    /// </summary>
    /// <returns>任务</returns>
    Task ProceedAsync();

    /// <summary>
    /// 获取参数
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    /// <param name="index">参数索引</param>
    /// <returns>参数值</returns>
    T GetArgument<T>(int index);

    /// <summary>
    /// 设置参数
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    /// <param name="index">参数索引</param>
    /// <param name="value">参数值</param>
    void SetArgument<T>(int index, T value);
}
