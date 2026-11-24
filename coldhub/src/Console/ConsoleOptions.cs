using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace ColdHub.Console;

public sealed class ConsoleOptions
{
    [Required, Url]
    public string AdminUrl { get; set; } = "";

    /// <summary>Console 调 Admin 用的管理员 key。它只存在于 Console 的服务端，浏览器永远拿不到。</summary>
    [Required, MinLength(8)]
    public string AdminKey { get; set; } = "";

    /// <summary>demo 用配置里的账号；真实部署应接入公司的统一登录。</summary>
    [Required, MinLength(1)]
    public List<ConsoleUser> Users { get; set; } = [];

    /// <summary>健康页要探测的各进程：名字 → 基础地址。</summary>
    public Dictionary<string, string> Services { get; set; } = [];
}

public sealed class ConsoleUser
{
    public string Name { get; set; } = "";
    public string Password { get; set; } = "";

    /// <summary>viewer 只读；admin 可以改。只分这两个角色。</summary>
    public string Role { get; set; } = Roles.Viewer;
}

public static class Roles
{
    public const string Viewer = "viewer";
    public const string Admin = "admin";
}

public static class Policies
{
    /// <summary>任何已登录的人。</summary>
    public const string Read = nameof(Read);

    /// <summary>只有 admin 角色：所有会改动数据的接口。</summary>
    public const string Write = nameof(Write);
}

[OptionsValidator]
public sealed partial class ConsoleOptionsValidator : IValidateOptions<ConsoleOptions>;
