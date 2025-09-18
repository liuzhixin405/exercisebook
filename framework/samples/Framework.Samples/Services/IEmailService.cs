namespace Framework.Samples.Services;

/// <summary>
/// 邮件服务接口
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// 发送邮件
    /// </summary>
    /// <param name="to">收件人</param>
    /// <param name="subject">主题</param>
    /// <param name="body">内容</param>
    /// <returns>是否发送成功</returns>
    Task<bool> SendEmailAsync(string to, string subject, string body);

    /// <summary>
    /// 发送欢迎邮件
    /// </summary>
    /// <param name="to">收件人</param>
    /// <param name="userName">用户名</param>
    /// <returns>是否发送成功</returns>
    Task<bool> SendWelcomeEmailAsync(string to, string userName);

    /// <summary>
    /// 发送密码重置邮件
    /// </summary>
    /// <param name="to">收件人</param>
    /// <param name="resetToken">重置令牌</param>
    /// <returns>是否发送成功</returns>
    Task<bool> SendPasswordResetEmailAsync(string to, string resetToken);
}
