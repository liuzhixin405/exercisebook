using Framework.Core.Abstractions.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Framework.Infrastructure.Decorators;

/// <summary>
/// 异常处理中间件 - 装饰器模式
/// 为中间件添加异常处理功能
/// </summary>
public class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly IExceptionHandler _exceptionHandler;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="exceptionHandler">异常处理器</param>
    public ExceptionHandlingMiddleware(IExceptionHandler exceptionHandler)
    {
        _exceptionHandler = exceptionHandler ?? throw new ArgumentNullException(nameof(exceptionHandler));
    }

    /// <inheritdoc />
    public string Name => "ExceptionHandling";

    /// <inheritdoc />
    public int Priority => 0; // 最高优先级，最先执行

    /// <inheritdoc />
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var handled = await _exceptionHandler.HandleExceptionAsync(ex, context);
            if (!handled)
            {
                // 如果异常处理器没有处理异常，重新抛出
                throw;
            }
        }
    }

    /// <inheritdoc />
    public bool ShouldExecute(HttpContext context)
    {
        return true; // 总是执行异常处理
    }
}
