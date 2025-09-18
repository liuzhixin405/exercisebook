using Framework.Core.Abstractions.Strategies;

namespace Framework.Samples.Strategies;

/// <summary>
/// 邮箱验证策略
/// </summary>
public class EmailValidationStrategy : IStrategy<bool>
{
    /// <inheritdoc />
    public string Name => "EmailValidationStrategy";

    /// <inheritdoc />
    public string Id => "email-validation";

    /// <inheritdoc />
    public int Priority => 100;

    /// <inheritdoc />
    public async Task<bool> ExecuteAsync(params object[] parameters)
    {
        if (parameters.Length == 0 || parameters[0] is not string email)
        {
            return false;
        }

        // 模拟验证时间
        await Task.Delay(10);

        // 简单的邮箱验证
        return IsValidEmail(email);
    }

    /// <inheritdoc />
    public bool CanExecute(params object[] parameters)
    {
        return parameters.Length > 0 && parameters[0] is string;
    }

    /// <summary>
    /// 验证邮箱格式
    /// </summary>
    /// <param name="email">邮箱地址</param>
    /// <returns>是否有效</returns>
    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

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
}
