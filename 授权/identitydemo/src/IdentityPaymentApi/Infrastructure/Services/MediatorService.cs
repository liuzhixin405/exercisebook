using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityPaymentApi.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityPaymentApi.Infrastructure.Services;

public class MediatorService(IServiceProvider serviceProvider) : IMediator
{
    public async Task<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest<TResponse>
    {
        var handler = serviceProvider.GetService<IRequestHandler<TRequest, TResponse>>();

        if (handler == null)
            throw new InvalidOperationException($"Handler not found for request type {request.GetType()}");

        var behaviors = serviceProvider.GetServices<IPipelineBehavior<TRequest, TResponse>>().Reverse();
        Task<TResponse> HandlerDelegate() => handler.Handle(request, cancellationToken);
        Func<Task<TResponse>> handlerDelegate = HandlerDelegate;

        foreach (var behavior in behaviors)
        {
            var next = handlerDelegate;
            handlerDelegate = () => behavior.Handle(request, next, cancellationToken);
        }

        return await handlerDelegate();
    }
}
