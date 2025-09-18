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
        // 这里使用反射创建代理
        // 在实际应用中，可以使用 Castle.DynamicProxy 或其他代理库
        return new ReflectionProxy<TInterface>(target, interceptors);
    }
}

/// <summary>
/// 反射代理实现
/// </summary>
/// <typeparam name="TInterface">接口类型</typeparam>
internal class ReflectionProxy<TInterface> where TInterface : class
{
    private TInterface? _target;
    private List<IInterceptor>? _interceptors;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="target">目标对象</param>
    /// <param name="interceptors">拦截器列表</param>
    public ReflectionProxy(TInterface target, List<IInterceptor> interceptors)
    {
        _target = target ?? throw new ArgumentNullException(nameof(target));
        _interceptors = interceptors ?? throw new ArgumentNullException(nameof(interceptors));
    }

    /// <summary>
    /// 调用方法
    /// </summary>
    /// <param name="methodName">方法名</param>
    /// <param name="args">参数</param>
    /// <returns>返回值</returns>
    public object? Invoke(string methodName, object[] args)
    {
        if (_target == null || _interceptors == null)
        {
            return null;
        }

        var method = typeof(TInterface).GetMethod(methodName);
        if (method == null)
        {
            return null;
        }

        var invocation = new Invocation(_target, method, args);
        
        // 执行拦截器链
        return ExecuteInterceptorsAsync(invocation).GetAwaiter().GetResult();
    }

    /// <summary>
    /// 执行拦截器链
    /// </summary>
    /// <param name="invocation">方法调用信息</param>
    /// <returns>执行结果</returns>
    private async Task<object?> ExecuteInterceptorsAsync(Invocation invocation)
    {
        var interceptorIndex = 0;

        async Task<object?> Next()
        {
            if (interceptorIndex >= _interceptors!.Count)
            {
                // 执行目标方法
                return invocation.Method.Invoke(invocation.Target, invocation.Arguments);
            }

            var interceptor = _interceptors[interceptorIndex++];
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

/// <summary>
/// 方法调用信息实现
/// </summary>
internal class Invocation : IInvocation
{
    private readonly Dictionary<string, object> _context;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="target">目标对象</param>
    /// <param name="method">方法信息</param>
    /// <param name="arguments">参数</param>
    public Invocation(object target, MethodInfo method, object[] arguments)
    {
        Target = target ?? throw new ArgumentNullException(nameof(target));
        Method = method ?? throw new ArgumentNullException(nameof(method));
        Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
        _context = new Dictionary<string, object>();
    }

    /// <inheritdoc />
    public object Target { get; }

    /// <inheritdoc />
    public MethodInfo Method { get; }

    /// <inheritdoc />
    public object[] Arguments { get; }

    /// <inheritdoc />
    public object? ReturnValue { get; set; }

    /// <inheritdoc />
    public bool IsHandled { get; set; }

    /// <inheritdoc />
    public IDictionary<string, object> Context => _context;

    /// <inheritdoc />
    public Task ProceedAsync()
    {
        // 这个方法在拦截器链中处理
        return Task.CompletedTask;
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public void SetArgument<T>(int index, T value)
    {
        if (index < 0 || index >= Arguments.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        Arguments[index] = value!;
    }
}
