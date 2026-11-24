using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace ColdHub.Source;

/// <summary>
/// 一个 Source 进程只服务一个已注册的源（Source__Name），用部署指定的连接器（Source__Connector）。
/// 想多接一个源就多起一个进程，不在一个进程里塞多个。
/// </summary>
public sealed class SourceOptions
{
    [Required]
    public string Name { get; set; } = "";

    [Required]
    public string Connector { get; set; } = "";

    [Required, Url]
    public string AdminUrl { get; set; } = "";

    [Required, MinLength(8)]
    public string ServiceKey { get; set; } = "";
}

[OptionsValidator]
public sealed partial class SourceOptionsValidator : IValidateOptions<SourceOptions>;
