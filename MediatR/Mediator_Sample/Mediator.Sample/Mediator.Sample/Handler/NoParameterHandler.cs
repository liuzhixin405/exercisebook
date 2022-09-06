using MediatR;

namespace Mediator.Sample.Handler
{

    public class NoParameter : IRequest { }           // in 参数必须要套个类型  IRequestHandler<IRequest, Unit>这样不会被识别到IRequest是接口无法实例化

    public class NoParameterHandler : IRequestHandler<NoParameter, Unit>
    {

        public Task<Unit> Handle(NoParameter request, CancellationToken cancellationToken)
        {
            Console.WriteLine("hello no parameter");
            return Task.FromResult(new Unit());
        }
    }
}
