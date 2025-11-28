using Framework.Core.Abstractions.Commands;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Framework.Extensions;

internal class CommandHandlerRegistrar : IHostedService
{
    private readonly IServiceProvider _provider;

    public CommandHandlerRegistrar(IServiceProvider provider)
    {
        _provider = provider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _provider.CreateScope();
        var services = scope.ServiceProvider;

        var commandBus = services.GetService<ICommandBus>();
        if (commandBus == null)
            return Task.CompletedTask;

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        var registerOne = typeof(ICommandBus).GetMethods()
            .First(m => m.Name == "RegisterHandler" && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 1);

        var registerTwo = typeof(ICommandBus).GetMethods()
            .First(m => m.Name == "RegisterHandler" && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == 2);

        foreach (var asm in assemblies)
        {
            Type[] types;
            try { types = asm.GetTypes(); } catch { continue; }

            foreach (var t in types.Where(t => !t.IsAbstract && !t.IsInterface))
            {
                foreach (var iface in t.GetInterfaces())
                {
                    if (!iface.IsGenericType) continue;

                    var def = iface.GetGenericTypeDefinition();
                    try
                    {
                        if (def == typeof(ICommandHandler<>))
                        {
                            var commandType = iface.GetGenericArguments()[0];
                            var genericRegister = registerOne.MakeGenericMethod(commandType);

                            // register a proxy that resolves the scoped handler per invocation
                            var proxyType = typeof(ServiceProviderHandlerProxy<>).MakeGenericType(commandType);
                            var proxy = Activator.CreateInstance(proxyType, _provider);
                            if (proxy != null)
                            {
                                try { genericRegister.Invoke(commandBus, new[] { proxy }); } catch { /* ignore */ }
                            }
                        }
                        else if (def == typeof(ICommandHandler<,>))
                        {
                            var genArgs = iface.GetGenericArguments();
                            var commandType = genArgs[0];
                            var resultType = genArgs[1];
                            var genericRegister = registerTwo.MakeGenericMethod(commandType, resultType);

                            var proxyType = typeof(ServiceProviderHandlerWithResultProxy<,>).MakeGenericType(commandType, resultType);
                            var proxy = Activator.CreateInstance(proxyType, _provider);
                            if (proxy != null)
                            {
                                try { genericRegister.Invoke(commandBus, new[] { proxy }); } catch { /* ignore */ }
                            }
                        }
                    }
                    catch
                    {
                        // ignore per-interface failures
                    }
                }
            }
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
