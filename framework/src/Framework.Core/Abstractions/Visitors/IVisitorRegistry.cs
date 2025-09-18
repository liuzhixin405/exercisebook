namespace Framework.Core.Abstractions.Visitors;

/// <summary>
/// 访问者注册器接口 - 访问者模式
/// 提供访问者注册和管理的抽象
/// </summary>
public interface IVisitorRegistry
{
    /// <summary>
    /// 注册访问者
    /// </summary>
    /// <typeparam name="TVisitable">可访问类型</typeparam>
    /// <param name="visitor">访问者</param>
    /// <returns>访问者注册器</returns>
    IVisitorRegistry RegisterVisitor<TVisitable>(IVisitor<TVisiTable> visitor)
        where TVisitable : class, IVisitable;

    /// <summary>
    /// 注册访问者（泛型）
    /// </summary>
    /// <typeparam name="TVisitable">可访问类型</typeparam>
    /// <typeparam name="TVisitor">访问者类型</typeparam>
    /// <returns>访问者注册器</returns>
    IVisitorRegistry RegisterVisitor<TVisitable, TVisitor>()
        where TVisitable : class, IVisitable
        where TVisitor : class, IVisitor<TVisiTable>;

    /// <summary>
    /// 移除访问者
    /// </summary>
    /// <typeparam name="TVisitable">可访问类型</typeparam>
    /// <param name="visitor">访问者</param>
    /// <returns>访问者注册器</returns>
    IVisitorRegistry RemoveVisitor<TVisitable>(IVisitor<TVisiTable> visitor)
        where TVisitable : class, IVisitable;

    /// <summary>
    /// 获取访问者
    /// </summary>
    /// <typeparam name="TVisitable">可访问类型</typeparam>
    /// <returns>访问者列表</returns>
    IEnumerable<IVisitor<TVisiTable>> GetVisitors<TVisitable>()
        where TVisitable : class, IVisitable;

    /// <summary>
    /// 清空所有访问者
    /// </summary>
    /// <returns>访问者注册器</returns>
    IVisitorRegistry Clear();

    /// <summary>
    /// 访问对象
    /// </summary>
    /// <typeparam name="TVisitable">可访问类型</typeparam>
    /// <param name="visitable">可访问对象</param>
    /// <returns>任务</returns>
    Task VisitAsync<TVisitable>(TVisitable visitable)
        where TVisitable : class, IVisitable;
}
