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
    public IVisitorRegistry RegisterVisitor<TVisitable>(IVisitor<TVisitable> visitor)
        where TVisitable : class, IVisitable
    {
        if (visitor == null)
            throw new ArgumentNullException(nameof(visitor));

        var visitableType = typeof(TVisitable);
        var genericVisitor = new VisitorWrapper<TVisitable>(visitor);

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
        where TVisitor : class, IVisitor<TVisitable>
    {
        // 这里需要服务提供者来创建访问者实例
        // 暂时抛出异常，提示需要服务提供者
        throw new InvalidOperationException("This method requires a service provider. Use the overload with service provider parameter.");
    }

    /// <inheritdoc />
    public IVisitorRegistry RemoveVisitor<TVisitable>(IVisitor<TVisitable> visitor)
        where TVisitable : class, IVisitable
    {
        if (visitor == null)
            return this;

        var visitableType = typeof(TVisitable);
        if (_visitors.TryGetValue(visitableType, out var visitors))
        {
            var wrapperToRemove = visitors.FirstOrDefault(v => v is VisitorWrapper<TVisitable> wrapper && wrapper.WrappedVisitorEquals(visitor));
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
    public IEnumerable<IVisitor<TVisitable>> GetVisitors<TVisitable>()
        where TVisitable : class, IVisitable
    {
        var visitableType = typeof(TVisitable);
        if (_visitors.TryGetValue(visitableType, out var visitors))
        {
            // Return adapters that implement IVisitor<TVisitable>
            return visitors.Select(v => new VisitorAdapter<TVisitable>(v)).OrderBy(v => v.Priority);
        }
        return Enumerable.Empty<IVisitor<TVisitable>>();
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

        var visitors = GetVisitors<TVisitable>();
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
/// 访问者包装器，只实现非泛型接口 IVisitor<IVisitable>
/// 将具体的 IVisitor<TVisitable> 包装为 IVisitor<IVisitable>
/// </summary>
internal class VisitorWrapper<TVisitable> : IVisitor<IVisitable> where TVisitable : class, IVisitable
{
    private readonly IVisitor<TVisitable> _wrappedVisitor;

    public VisitorWrapper(IVisitor<TVisitable> wrappedVisitor)
    {
        _wrappedVisitor = wrappedVisitor ?? throw new ArgumentNullException(nameof(wrappedVisitor));
    }

    public IVisitor<TVisitable> WrappedVisitor => _wrappedVisitor;

    public bool WrappedVisitorEquals(IVisitor<TVisitable> other)
    {
        return ReferenceEquals(_wrappedVisitor, other);
    }

    public string Name => _wrappedVisitor.Name;

    public int Priority => _wrappedVisitor.Priority;

    public async Task VisitAsync(IVisitable visitable)
    {
        if (visitable is TVisitable typed)
        {
            await _wrappedVisitor.VisitAsync(typed);
        }
    }

    public bool ShouldVisit(IVisitable visitable)
    {
        if (visitable is TVisitable typed)
        {
            return _wrappedVisitor.ShouldVisit(typed);
        }
        return false;
    }
}

/// <summary>
/// 适配器：将 IVisitor<IVisitable> 适配为 IVisitor<TVisitable>
/// </summary>
internal class VisitorAdapter<TVisitable> : IVisitor<TVisitable> where TVisitable : class, IVisitable
{
    private readonly IVisitor<IVisitable> _inner;

    public VisitorAdapter(IVisitor<IVisitable> inner)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
    }

    public string Name => _inner.Name;

    public int Priority => _inner.Priority;

    public Task VisitAsync(TVisitable visitable)
    {
        return _inner.VisitAsync(visitable);
    }

    public bool ShouldVisit(TVisitable visitable)
    {
        return _inner.ShouldVisit(visitable);
    }
}
