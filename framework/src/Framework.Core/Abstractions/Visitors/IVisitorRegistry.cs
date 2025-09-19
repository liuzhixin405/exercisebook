namespace Framework.Core.Abstractions.Visitors;

/// <summary>
/// 访问者注册器接口 - 访问者模�?
/// 提供访问者注册和管理的抽�?
/// </summary>
public interface IVisitorRegistry
{
    /// <summary>
    /// 注册访问�?
    /// </summary>
    /// <typeparam name="TVisitable">可访问类�?/typeparam>
    /// <param name="visitor">访问�?/param>
    /// <returns>访问者注册器</returns>
    IVisitorRegistry RegisterVisitor<TVisitable>(IVisitor<TVisitable> visitor)
        where TVisitable : class, IVisitable;

    /// <summary>
    /// 注册访问者（泛型�?
    /// </summary>
    /// <typeparam name="TVisitable">可访问类�?/typeparam>
    /// <typeparam name="TVisitor">访问者类�?/typeparam>
    /// <returns>访问者注册器</returns>
    IVisitorRegistry RegisterVisitor<TVisitable, TVisitor>()
        where TVisitable : class, IVisitable
        where TVisitor : class, IVisitor<TVisitable>;

    /// <summary>
    /// 移除访问�?
    /// </summary>
    /// <typeparam name="TVisitable">可访问类�?/typeparam>
    /// <param name="visitor">访问�?/param>
    /// <returns>访问者注册器</returns>
    IVisitorRegistry RemoveVisitor<TVisitable>(IVisitor<TVisitable> visitor)
        where TVisitable : class, IVisitable;

    /// <summary>
    /// 获取访问�?
    /// </summary>
    /// <typeparam name="TVisitable">可访问类�?/typeparam>
    /// <returns>访问者列�?/returns>
    IEnumerable<IVisitor<IVisitable>> GetVisitors<TVisitable>()
        where TVisitable : class, IVisitable;

    /// <summary>
    /// 清空所有访问�?
    /// </summary>
    /// <returns>访问者注册器</returns>
    IVisitorRegistry Clear();

    /// <summary>
    /// 访问对象
    /// </summary>
    /// <typeparam name="TVisitable">可访问类�?/typeparam>
    /// <param name="visitable">可访问对�?/param>
    /// <returns>任务</returns>
    Task VisitAsync<TVisitable>(TVisitable visitable)
        where TVisitable : class, IVisitable;
}
