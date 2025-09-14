using Common.Bus.Core;

namespace WebApp.Commands
{
    /// <summary>
    /// 创建用户命令
    /// </summary>
    /// <param name="Name">用户名</param>
    /// <param name="Email">邮箱</param>
    /// <param name="Age">年龄</param>
    public record CreateUserCommand(string Name, string Email, int Age) : ICommand<int>;
}
