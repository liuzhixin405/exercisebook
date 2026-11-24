using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using ColdHub.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace ColdHub.Admin;

/// <summary>Options 模式 + 校验：启动时就失败，而不是等用到才发现配置缺失（对应 CLAUDE.md 的配置约定）。</summary>
public sealed class AdminOptions
{
    /// <summary>Source、Hub 这些程序带着它调 Admin（对应 Admin__ServiceKey）。</summary>
    [Required, MinLength(8)]
    public string ServiceKey { get; set; } = "";

    /// <summary>管理员（人通过 Console 操作时）用的 key。只有它能改配置、建 key。</summary>
    [Required, MinLength(8)]
    public string AdminKey { get; set; } = "";

    [Required]
    public string BootstrapClientKey { get; set; } = "";

    [Required]
    public string DataFile { get; set; } = "";
}

[OptionsValidator]
public sealed partial class AdminOptionsValidator : IValidateOptions<AdminOptions>;

public static class Roles
{
    public const string Service = "service";
    public const string Admin = "admin";
}

public static class Policies
{
    /// <summary>程序之间调用：service 或 admin 的 key 都行。</summary>
    public const string Service = nameof(Service);

    /// <summary>改配置、建 key：只有 admin。</summary>
    public const string Admin = nameof(Admin);
}

/// <summary>
/// 读 <c>Authorization: ApiKey xxx</c>，和配置里的两把 key 比较，通过后给出角色。
/// 比较用固定时间算法，避免通过响应时间猜 key。
/// </summary>
public sealed class ApiKeyHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger,
    UrlEncoder encoder, IOptions<AdminOptions> admin) : AuthenticationHandler<AuthenticationSchemeOptions>(options,
    logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string? key = ApiKeys.FromHeader(Request.Headers.Authorization);

        if (key is null)
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        string? role = Matches(key, admin.Value.AdminKey) ? Roles.Admin
            : Matches(key, admin.Value.ServiceKey) ? Roles.Service
            : null;

        if (role is null)
        {
            return Task.FromResult(AuthenticateResult.Fail("unknown key"));
        }

        var identity = new ClaimsIdentity([new Claim(ClaimTypes.Role, role)], Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identity),
            Scheme.Name)));
    }

    private static bool Matches(string given, string expected) =>
        CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(given), Encoding.UTF8.GetBytes(expected));
}
