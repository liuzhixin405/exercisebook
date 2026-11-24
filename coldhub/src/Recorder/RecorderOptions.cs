using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace ColdHub.Recorder;

public sealed class RecorderOptions
{
    /// <summary>历史文件目录：{HistoryDir}/{source}/{yyyy-MM-dd}.csv.gz。Hub 的 HistoryDir 要指向同一处。</summary>
    [Required]
    public string HistoryDir { get; set; } = "";
}

[OptionsValidator]
public sealed partial class RecorderOptionsValidator : IValidateOptions<RecorderOptions>;
