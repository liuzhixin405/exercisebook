using MediatR;

namespace MediatRDemo.Model
{
    /// <summary>
    /// 一对一消息
    /// </summary>
    public class CreateUserCommand : IRequest<string>
    {
        public string Name { get; set; }
    }
}
