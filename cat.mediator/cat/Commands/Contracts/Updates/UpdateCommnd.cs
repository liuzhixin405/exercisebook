using MediatR;

namespace cat.Commands.Contracts.Updates
{
    public class UpdateCommnd:IRequest
    {
        public UpdateDto dto { get; set; }  
    }
}
