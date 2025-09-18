using Framework.Core.Abstractions.Visitors;
using System.Collections.Concurrent;

namespace Framework.Infrastructure.Visitors;

/// <summary>
/// 访问者注册器实现 - 访问者模式
/// 提供访问者注册和管理的实现
/// </summary>
public class VisitorRegistry : IVisitorRegistry
{
    private readonly ConcurrentDictionary<Type, List<IVisitor<IVisitable>>> _visitors;

    /// <summary>
    /// 构造函数
    /// </summary>
    public VisitorRegistry()
    {
        _visitors = new ConcurrentDictionary<Type, List<IVisitor<IVisitable>>>();
    }

    /// <inheritdoc />
    public IVisitorRegistry RegisterVisitor<TVisitable>(IVisitor<TVisiTable> visitor)
        where TVisitable : class, IVisitable
    {
        if (visitor == null)
            throw new ArgumentNullException(nameof(visitor));

        var visitableType = typeof(TVisiTable);
        var genericVisitor = new VisitorWrapper<TVisiTable>(visitor);

        _visitors.AddOrUpdate(
            visitableType,
            new List<IVisitor<IVisitable>> { genericVisitor },
            (key, existing) =>
            {
                existing.Add(genericVisitor);
                return existing;
            });

        return this;
    }

    /// <inheritdoc />
    public IVisitorRegistry RegisterVisitor<TVisitable, TVisitor>()
        where TVisitable : class, IVisitable
        where TVisitor : class, IVisitor<TVisiTable>
    {
        // 这里需要服务提供者来创建访问者实例
        // 暂时抛出异常，提示需要服务提供者
        throw new InvalidOperationException("This method requires a service provider. Use the overload with service provider parameter.");
    }

    /// <inheritdoc />
    public IVisitorRegistry RemoveVisitor<TVisitable>(IVisitor<TVisiTable> visitor)
        where TVisitable : class, IVisitable
    {
        if (visitor == null)
            return this;

        var visitableType = typeof(TVisiTable);
        if (_visitors.TryGetValue(visitableType, out var visitors))
        {
            var wrapperToRemove = visitors.FirstOrDefault(v => v is VisitorWrapper<TVisiTable> wrapper && wrapper.WrappedVisitor == visitor);
            if (wrapperToRemove != null)
            {
                visitors.Remove(wrapperToRemove);
                if (visitors.Count == 0)
                {
                    _visitors.TryRemove(visitableType, out _);
                }
            }
        }

        return this;
    }

    /// <inheritdoc />
    public IEnumerable<IVisitor<IVisitable>> GetVisitors<TVisitable>()
        where TVisitable : class, IVisitable
    {
        var visitableType = typeof(TVisiTable);
        if (_visitors.TryGetValue(visitableType, out var visitors))
        {
            return visitors.OrderBy(v => v.Priority);
        }
        return Enumerable.Empty<IVisitor<IVisitable>>();
    }

    /// <inheritdoc />
    public IVisitorRegistry Clear()
    {
        _visitors.Clear();
        return this;
    }

    /// <inheritdoc />
    public async Task VisitAsync<TVisitable>(TVisitable visitable)
        where TVisitable : class, IVisitable
    {
        if (visitable == null)
            throw new ArgumentNullException(nameof(visitable));

        var visitors = GetVisitors<TVisiTable>();
        foreach (var visitor in visitors)
        {
            if (visitor.ShouldVisit(visitable))
            {
                await visitor.VisitAsync(visitable);
            }
        }
    }
}

/// <summary>
/// 访问者包装器
/// </summary>
/// <typeparam name="TVisitable">可访问类型</typeparam>
internal class VisitorWrapper<TVisitable> : IVisitor<IVisitable> where TVisitable : class, IVisitable
{
    private readonly IVisitor<TVisiTable> _wrappedVisitor;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="wrappedVisitor">被包装的访问者</param>
    public VisitorWrapper(IVisitor<TVisiTable> wrappedVisitor)
    {
        _wrappedVisitor = wrappedVisitor ?? throw new ArgumentNullException(nameof(wrappedVisitor));
    }

    /// <summary>
    /// 获取被包装的访问者
    /// </summary>
    public IVisitor<TVisiTable> WrappedVisitor => _wrappedVisitor;

    /// <inheritdoc />
    public string Name => _wrappedVisitor.Name;

    /// <inheritdoc />
    public int Priority => _wrappedVisitor.Priority;

    /// <inheritdoc />
    public async Task VisitAsync(IVisitable visitable)
    {
        if (visitable is TVisitable typedVisitable)
        {
            await _wrappedVisitor.VisitAsync(typedVisitable);
        }
    }

    /// <inheritdoc />
    public bool ShouldVisit(IVisitable visitable)
    {
        if (visitable is TVisitable typedVisitable)
        {
            return _wrappedVisitor.ShouldVisit(typedVisitable);
        }
        return false;
    }
}
