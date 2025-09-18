namespace Framework.Samples.Services;

/// <summary>
/// 邮件服务实现
/// </summary>
public class EmailService : IEmailService
{
    /// <inheritdoc />
    public async Task<bool> SendEmailAsync(string to, string subject, string body)
    {
        // 模拟发送邮件
        await Task.Delay(100);
        Console.WriteLine($"发送邮件到: {to}");
        Console.WriteLine($"主题: {subject}");
        Console.WriteLine($"内容: {body}");
        Console.WriteLine("---");
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> SendWelcomeEmailAsync(string to, string userName)
    {
        var subject = "欢迎注册";
        var body = $"亲爱的 {userName}，欢迎您注册我们的服务！";
        return await SendEmailAsync(to, subject, body);
    }

    /// <inheritdoc />
    public async Task<bool> SendPasswordResetEmailAsync(string to, string resetToken)
    {
        var subject = "密码重置";
        var body = $"您的密码重置令牌是: {resetToken}";
        return await SendEmailAsync(to, subject, body);
    }
}
