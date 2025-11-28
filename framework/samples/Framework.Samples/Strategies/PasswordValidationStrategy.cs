using Framework.Core.Abstractions.Strategies;

namespace Framework.Samples.Strategies;

/// <summary>
/// 密码验证策略
/// </summary>
public class PasswordValidationStrategy : IStrategy<bool>
{
    /// <inheritdoc />
    public string Name => "PasswordValidationStrategy";

    /// <inheritdoc />
    public string Id => "password-validation";

    /// <inheritdoc />
    public int Priority => 100;

    /// <inheritdoc />
    public async Task<bool> ExecuteAsync(params object[] parameters)
    {
        if (parameters.Length == 0 || parameters[0] is not string password)
        {
            return false;
        }

        // 模拟验证时间
        await Task.Delay(10);

        // 密码验证规则
        return IsValidPassword(password);
    }

    // Explicit non-generic interface implementation
    async Task<object?> Framework.Core.Abstractions.Strategies.IStrategy.ExecuteAsync(params object[] parameters)
    {
        var result = await ExecuteAsync(parameters);
        return (object?)result;
    }

    /// <inheritdoc />
    public bool CanExecute(params object[] parameters)
    {
        return parameters.Length > 0 && parameters[0] is string;
    }

    /// <summary>
    /// 验证密码强度
    /// </summary>
    /// <param name="password">密码</param>
    /// <returns>是否有效</returns>
    private static bool IsValidPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        // 密码长度至少8位
        if (password.Length < 8)
            return false;

        // 包含至少一个大写字母
        if (!password.Any(char.IsUpper))
            return false;

        // 包含至少一个小写字母
        if (!password.Any(char.IsLower))
            return false;

        // 包含至少一个数字
        if (!password.Any(char.IsDigit))
            return false;

        return true;
    }
}
