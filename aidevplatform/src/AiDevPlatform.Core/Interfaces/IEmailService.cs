namespace AiDevPlatform.Core.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
    Task SendEmailAsync(string to, string cc, string subject, string body, bool isHtml = true);
    Task SendEmailAsync(List<string> to, List<string> cc, string subject, string body, bool isHtml = true);
    Task SendPasswordResetEmailAsync(string email, string resetToken);
    Task SendWelcomeEmailAsync(string email, string userName);
    Task SendNotificationEmailAsync(string email, string subject, string message);
    Task<bool> IsValidEmailAsync(string email);
}
