using MediatR;
using MediatRDemo.Handler;
using MediatRDemo.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MediatRDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediatRTestController : ControllerBase
    {
       private readonly IMediator _mediator;
        public MediatRTestController(IMediator mediator)
        {
            _mediator=mediator;
        }
        /// <summary>
        /// 生产端 只会出现在CreateUserHandler中出现，实现一对一
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpPost("User")]
        public async Task<string> CreateUserAsync([FromQuery] string name)
        {
            var response = await _mediator.Send(new CreateUserCommand { Name = name });
            return response;
        }

        /// <summary>
        /// 会在输出栏打印 First和Second 两次的内容  实现广播
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpPost("Publish")]
        public async Task PublishNotificationAsync([FromQuery] string name)
        {
            await _mediator.Publish(new MyNotificationCommand { Message = name });
        }
    }
}

/*
  无返回值消息处理
   //消息
    public class NoResponseCommand : IRequest { }

    //处理器
    public class NoResponseHandler : AsyncRequestHandler<NoResponseCommand>
    {
        protected override async Task Handle(NoResponseCommand request, CancellationToken cancellationToken)
        {
            //handle the logic
        }
    }

    //接口
    [HttpPost("NoResponse")]
    public async Task NoResponseAsync()
    {
        await _mediator.Send(new NoResponseCommand());
    }
 */
