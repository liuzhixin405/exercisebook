namespace Framework.Infrastructure.Templates;

/// <summary>
/// 模板方法接口 - 模板方法模式
/// 提供算法骨架的抽象
/// </summary>
/// <typeparam name="TContext">上下文类型</typeparam>
/// <typeparam name="TResult">结果类型</typeparam>
public interface ITemplateMethod<TContext, TResult>
{
    /// <summary>
    /// 模板方法名称
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 执行模板方法
    /// </summary>
    /// <param name="context">上下文</param>
    /// <returns>结果</returns>
    Task<TResult> ExecuteAsync(TContext context);

    /// <summary>
    /// 初始化（钩子方法）
    /// </summary>
    /// <param name="context">上下文</param>
    /// <returns>任务</returns>
    Task InitializeAsync(TContext context);

    /// <summary>
    /// 验证（钩子方法）
    /// </summary>
    /// <param name="context">上下文</param>
    /// <returns>是否有效</returns>
    Task<bool> ValidateAsync(TContext context);

    /// <summary>
    /// 处理（抽象方法）
    /// </summary>
    /// <param name="context">上下文</param>
    /// <returns>结果</returns>
    Task<TResult> ProcessAsync(TContext context);

    /// <summary>
    /// 清理（钩子方法）
    /// </summary>
    /// <param name="context">上下文</param>
    /// <param name="result">结果</param>
    /// <returns>任务</returns>
    Task CleanupAsync(TContext context, TResult result);
}

/// <summary>
/// 模板方法基类 - 模板方法模式
/// </summary>
/// <typeparam name="TContext">上下文类型</typeparam>
/// <typeparam name="TResult">结果类型</typeparam>
public abstract class TemplateMethodBase<TContext, TResult> : ITemplateMethod<TContext, TResult>
{
    /// <inheritdoc />
    public abstract string Name { get; }

    /// <inheritdoc />
    public async Task<TResult> ExecuteAsync(TContext context)
    {
        try
        {
            // 1. 初始化
            await InitializeAsync(context);

            // 2. 验证
            if (!await ValidateAsync(context))
            {
                throw new InvalidOperationException("Validation failed");
            }

            // 3. 处理
            var result = await ProcessAsync(context);

            // 4. 清理
            await CleanupAsync(context, result);

            return result;
        }
        catch (Exception ex)
        {
            await OnErrorAsync(context, ex);
            throw;
        }
    }

    /// <inheritdoc />
    public virtual Task InitializeAsync(TContext context)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual Task<bool> ValidateAsync(TContext context)
    {
        return Task.FromResult(true);
    }

    /// <inheritdoc />
    public abstract Task<TResult> ProcessAsync(TContext context);

    /// <inheritdoc />
    public virtual Task CleanupAsync(TContext context, TResult result)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// 错误处理（钩子方法）
    /// </summary>
    /// <param name="context">上下文</param>
    /// <param name="exception">异常</param>
    /// <returns>任务</returns>
    protected virtual Task OnErrorAsync(TContext context, Exception exception)
    {
        return Task.CompletedTask;
    }
}
