using Common.Bus.Core;

namespace WebApp.Handlers.Commands
{
    // 命令和请求模型
    public record ProcessOrderCommand(string Product, int Quantity, int Priority = 1) : ICommand<string>;
}
