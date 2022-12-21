using MediatR;

namespace cat.Commands.Contracts.Creates
{
    public class CreateCommand : IRequest
    {
        public CreateDTo dto { get; set; }
    }
}
