using Framework.Core.Abstractions.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Framework.Extensions;

/// <summary>
/// Per-call proxies that resolve the real handler from a scoped provider on each invocation.
/// These live in Framework.Extensions so the registrar can create instances without referencing internal types in Infrastructure.
/// </summary>
internal class ServiceProviderHandlerProxy<TCommand> : ICommandHandler<TCommand> where TCommand : class, Framework.Core.Abstractions.Commands.ICommand
{
    private readonly IServiceProvider _provider;

    public ServiceProviderHandlerProxy(IServiceProvider provider)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    public string Name => "ServiceProviderProxy";

    public int Priority => 100;

    public async Task HandleAsync(TCommand command)
    {
        using var scope = _provider.CreateScope();
        var handler = (ICommandHandler<TCommand>)scope.ServiceProvider.GetRequiredService(typeof(ICommandHandler<TCommand>));
        await handler.HandleAsync(command);
    }

    public bool ShouldHandle(TCommand command)
    {
        using var scope = _provider.CreateScope();
        var handler = (ICommandHandler<TCommand>)scope.ServiceProvider.GetRequiredService(typeof(ICommandHandler<TCommand>));
        return handler.ShouldHandle(command);
    }
}

internal class ServiceProviderHandlerWithResultProxy<TCommand, TResult> : ICommandHandler<TCommand, TResult> where TCommand : class, Framework.Core.Abstractions.Commands.ICommand<TResult>
{
    private readonly IServiceProvider _provider;

    public ServiceProviderHandlerWithResultProxy(IServiceProvider provider)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    public string Name => "ServiceProviderProxy";

    public int Priority => 100;

    public async Task<TResult> HandleAsync(TCommand command)
    {
        using var scope = _provider.CreateScope();
        var handler = (ICommandHandler<TCommand, TResult>)scope.ServiceProvider.GetRequiredService(typeof(ICommandHandler<TCommand, TResult>));
        return await handler.HandleAsync(command);
    }

    public bool ShouldHandle(TCommand command)
    {
        using var scope = _provider.CreateScope();
        var handler = (ICommandHandler<TCommand, TResult>)scope.ServiceProvider.GetRequiredService(typeof(ICommandHandler<TCommand, TResult>));
        return handler.ShouldHandle(command);
    }
}
