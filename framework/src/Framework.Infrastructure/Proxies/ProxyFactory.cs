using Framework.Core.Abstractions.Proxies;
using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;

namespace Framework.Infrastructure.Proxies;

/// <summary>
/// 代理工厂实现 - 代理模式
/// 提供创建代理对象的实现
/// </summary>
public class ProxyFactory : IProxyFactory
{
    private readonly List<IInterceptor> _globalInterceptors;
    private readonly object _lockObject = new object();

    /// <summary>
    /// 构造函数
    /// </summary>
    public ProxyFactory()
    {
        _globalInterceptors = new List<IInterceptor>();
    }

    /// <inheritdoc />
    public TInterface CreateProxy<TInterface>(TInterface target, params IInterceptor[] interceptors)
        where TInterface : class
    {
        if (target == null)
            throw new ArgumentNullException(nameof(target));

        var allInterceptors = CombineInterceptors(interceptors);
        return CreateProxyInternal<TInterface>(target, allInterceptors);
    }

    /// <inheritdoc />
    public TInterface CreateProxy<TInterface>(Func<TInterface> factory, params IInterceptor[] interceptors)
        where TInterface : class
    {
        if (factory == null)
            throw new ArgumentNullException(nameof(factory));

        var target = factory();
        return CreateProxy(target, interceptors);
    }

    /// <inheritdoc />
    public TInterface CreateProxy<TInterface>(Func<object[], TInterface> factory, object[] parameters, params IInterceptor[] interceptors)
        where TInterface : class
    {
        if (factory == null)
            throw new ArgumentNullException(nameof(factory));

        var target = factory(parameters);
        return CreateProxy(target, interceptors);
    }

    /// <inheritdoc />
    public IProxyFactory RegisterInterceptor(IInterceptor interceptor)
    {
        if (interceptor == null)
            throw new ArgumentNullException(nameof(interceptor));

        lock (_lockObject)
        {
            _globalInterceptors.Add(interceptor);
        }
        return this;
    }

    /// <inheritdoc />
    public IProxyFactory RemoveInterceptor(IInterceptor interceptor)
    {
        if (interceptor == null)
            return this;

        lock (_lockObject)
        {
            _globalInterceptors.Remove(interceptor);
        }
        return this;
    }

    /// <inheritdoc />
    public IProxyFactory ClearInterceptors()
    {
        lock (_lockObject)
        {
            _globalInterceptors.Clear();
        }
        return this;
    }

    /// <summary>
    /// 合并拦截器
    /// </summary>
    /// <param name="interceptors">拦截器数组</param>
    /// <returns>合并后的拦截器列表</returns>
    private List<IInterceptor> CombineInterceptors(IInterceptor[] interceptors)
    {
        var allInterceptors = new List<IInterceptor>();

        // 添加全局拦截器
        lock (_lockObject)
        {
            allInterceptors.AddRange(_globalInterceptors);
        }

        // 添加特定拦截器
        if (interceptors != null)
        {
            allInterceptors.AddRange(interceptors);
        }

        // 按优先级排序
        return allInterceptors.OrderBy(i => i.Priority).ToList();
    }

    /// <summary>
    /// 创建代理内部实现
    /// </summary>
    /// <typeparam name="TInterface">接口类型</typeparam>
    /// <param name="target">目标对象</param>
    /// <param name="interceptors">拦截器列表</param>
    /// <returns>代理对象</returns>
    private TInterface CreateProxyInternal<TInterface>(TInterface target, List<IInterceptor> interceptors)
        where TInterface : class
    {
        // Use DispatchProxy to create a proxy implementing TInterface
        var proxy = DispatchProxy.Create<TInterface, DispatchProxyHandler<TInterface>>();
        var handler = (DispatchProxyHandler<TInterface>)(object)proxy!;
        handler.SetParameters(target!, interceptors);
        return proxy as TInterface ?? target;
    }
}

internal class DispatchProxyHandler<TInterface> : DispatchProxy where TInterface : class
{
    private TInterface? _target;
    private List<IInterceptor>? _interceptors;

    public void SetParameters(TInterface target, List<IInterceptor> interceptors)
    {
        _target = target;
        _interceptors = interceptors;
    }

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        if (targetMethod == null || _target == null)
            throw new InvalidOperationException("Invalid proxy invocation");

        var invocation = new Invocation(_target, targetMethod, args ?? Array.Empty<object>());

        // Synchronous path: run interceptors synchronously by awaiting tasks
        var task = ExecuteInterceptorsAsync(invocation);
        return task.GetAwaiter().GetResult();
    }

    private async Task<object?> ExecuteInterceptorsAsync(Invocation invocation)
    {
        var interceptorIndex = 0;

        async Task<object?> Next()
        {
            if (interceptorIndex >= (_interceptors?.Count ?? 0))
            {
                return invocation.Method.Invoke(invocation.Target, invocation.Arguments);
            }

            var interceptor = _interceptors![interceptorIndex++];
            if (interceptor.ShouldIntercept(invocation))
            {
                await interceptor.InterceptAsync(invocation);
            }

            if (invocation.IsHandled)
            {
                return invocation.ReturnValue;
            }

            return await Next();
        }

        return await Next();
    }
}

internal class Invocation : Framework.Core.Abstractions.Proxies.IInvocation
{
    private readonly Dictionary<string, object> _context;

    public Invocation(object target, MethodInfo method, object[] arguments)
    {
        Target = target ?? throw new ArgumentNullException(nameof(target));
        Method = method ?? throw new ArgumentNullException(nameof(method));
        Arguments = arguments ?? Array.Empty<object>();
        _context = new Dictionary<string, object>();
    }

    public object Target { get; }

    public MethodInfo Method { get; }

    public object[] Arguments { get; }

    public object? ReturnValue { get; set; }

    public bool IsHandled { get; set; }

    public IDictionary<string, object> Context => _context;

    public Task ProceedAsync()
    {
        // invocation proceeds by calling the target method synchronously when there are no more interceptors
        var result = Method.Invoke(Target, Arguments);
        ReturnValue = result;
        return Task.CompletedTask;
    }

    public T GetArgument<T>(int index)
    {
        if (index < 0 || index >= Arguments.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        if (Arguments[index] is T typedValue)
        {
            return typedValue;
        }

        throw new InvalidCastException($"Argument at index {index} cannot be cast to {typeof(T).Name}");
    }

    public void SetArgument<T>(int index, T value)
    {
        if (index < 0 || index >= Arguments.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        Arguments[index] = value!;
    }
}
