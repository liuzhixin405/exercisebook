namespace Framework.Core.Abstractions.Visitors;

/// <summary>
/// 访问者接口 - 访问者模式
/// 提供访问操作的抽象
/// </summary>
/// <typeparam name="TVisitable">可访问类型</typeparam>
public interface IVisitor<in TVisitable> where TVisitable : class, IVisitable
{
    /// <summary>
    /// 访问者名称
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 访问者优先级（数字越小优先级越高）
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// 访问对象
    /// </summary>
    /// <param name="visitable">可访问对象</param>
    /// <returns>任务</returns>
    Task VisitAsync(TVisitable visitable);

    /// <summary>
    /// 是否应该访问此对象
    /// </summary>
    /// <param name="visitable">可访问对象</param>
    /// <returns>是否应该访问</returns>
    bool ShouldVisit(TVisitable visitable);
}
