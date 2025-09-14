using Common.Bus.Core;

namespace WebApp.Commands
{
    /// <summary>
    /// 发送邮件命令
    /// </summary>
    /// <param name="To">收件人</param>
    /// <param name="Subject">主题</param>
    /// <param name="Body">内容</param>
    public record SendEmailCommand(string To, string Subject, string Body) : ICommand<bool>;
}
