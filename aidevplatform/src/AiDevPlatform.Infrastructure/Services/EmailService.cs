using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using AiDevPlatform.Core.Interfaces;

namespace AiDevPlatform.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IConfiguration _configuration;

    public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false)
    {
        try
        {
            _logger.LogInformation("Sending email to {Email} with subject {Subject}", to, subject);

            // TODO: Implement actual email sending logic
            // This is a placeholder implementation
            await Task.Delay(100); // Simulate email sending

            _logger.LogInformation("Email sent successfully to {Email}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", to);
            throw;
        }
    }

    public async Task SendEmailAsync(string to, string cc, string subject, string body, bool isHtml = true)
    {
        try
        {
            _logger.LogInformation("Sending email to {Email} with CC {CC} and subject {Subject}", to, cc, subject);

            // TODO: Implement actual email sending logic
            await Task.Delay(100); // Simulate email sending

            _logger.LogInformation("Email sent successfully to {Email}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", to);
            throw;
        }
    }

    public async Task SendEmailAsync(List<string> to, List<string> cc, string subject, string body, bool isHtml = true)
    {
        try
        {
            _logger.LogInformation("Sending email to {Recipients} with CC {CC} and subject {Subject}",
                string.Join(", ", to), string.Join(", ", cc), subject);

            // TODO: Implement actual email sending logic
            await Task.Delay(100); // Simulate email sending

            _logger.LogInformation("Email sent successfully to {Recipients}", string.Join(", ", to));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Recipients}", string.Join(", ", to));
            throw;
        }
    }

    public async Task<bool> IsValidEmailAsync(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public async Task SendWelcomeEmailAsync(string email, string userName)
    {
        var subject = "Welcome to AI Development Platform";
        var body = $@"
            <html>
            <body>
                <h2>Welcome to AI Development Platform</h2>
                <p>Hello {userName},</p>
                <p>Welcome to our low code platform! Your account has been created successfully.</p>
                <p>You can now start building applications using our visual designer.</p>
                <br>
                <p>Best regards,<br>AI Development Platform Team</p>
            </body>
            </html>";

        await SendEmailAsync(email, subject, body, true);
    }

    public async Task SendPasswordResetEmailAsync(string email, string resetToken)
    {
        var subject = "Password Reset - AI Development Platform";
        var body = $@"
            <html>
            <body>
                <h2>Password Reset</h2>
                <p>Hello,</p>
                <p>You have requested to reset your password. Please click the link below to reset your password.</p>
                <p><a href='{_configuration["App:BaseUrl"]}/reset-password?token={resetToken}'>Reset Password</a></p>
                <p>If you did not request a password reset, please ignore this email.</p>
                <p>This link will expire in 24 hours.</p>
                <br>
                <p>Best regards,<br>AI Development Platform Team</p>
            </body>
            </html>";

        await SendEmailAsync(email, subject, body, true);
    }

    public async Task SendNotificationEmailAsync(string email, string title, string message)
    {
        var subject = $"Notification: {title}";
        var body = $@"
            <html>
            <body>
                <h2>{title}</h2>
                <p>{message}</p>
                <br>
                <p>Best regards,<br>AI Development Platform Team</p>
            </body>
            </html>";

        await SendEmailAsync(email, subject, body, true);
    }
}