using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace ColdHub.Hub;

public sealed class HubOptions
{
    [Required, Url]
    public string AdminUrl { get; set; } = "";

    [Required, MinLength(8)]
    public string ServiceKey { get; set; } = "";

    /// <summary>历史文件目录，与 Recorder 的 HistoryDir 指向同一处。</summary>
    [Required]
    public string HistoryDir { get; set; } = "";
}

[OptionsValidator]
public sealed partial class HubOptionsValidator : IValidateOptions<HubOptions>;
