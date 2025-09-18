namespace Framework.Core.Abstractions.Visitors;

/// <summary>
/// 可访问接口 - 访问者模式
/// 提供接受访问者的抽象
/// </summary>
public interface IVisitable
{
    /// <summary>
    /// 接受访问者
    /// </summary>
    /// <param name="visitor">访问者</param>
    /// <returns>任务</returns>
    Task AcceptAsync(IVisitor<IVisitable> visitor);

    /// <summary>
    /// 对象标识
    /// </summary>
    string Id { get; }

    /// <summary>
    /// 对象类型
    /// </summary>
    string Type { get; }
}
