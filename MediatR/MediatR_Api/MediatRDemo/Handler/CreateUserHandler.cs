using MediatR;
using MediatRDemo.Model;
using System.Threading;
using System.Threading.Tasks;

namespace MediatRDemo.Handler
{
    /// <summary>
    /// 消费端
    /// </summary>
    public class CreateUserHandler : IRequestHandler<CreateUserCommand, string>
    {
        public async Task<string> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            return await Task.FromResult($"New name is {request.Name}");
        }
    }
}
