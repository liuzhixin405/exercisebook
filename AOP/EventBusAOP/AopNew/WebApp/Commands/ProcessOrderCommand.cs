using Common.Bus.Core;

namespace WebApp.Commands
{
    /// <summary>
    /// 处理订单命令
    /// </summary>
    /// <param name="Product">产品名称</param>
    /// <param name="Quantity">数量</param>
    /// <param name="Priority">优先级</param>
    public record ProcessOrderCommand(string Product, int Quantity, int Priority = 1) : ICommand<string>;
}
