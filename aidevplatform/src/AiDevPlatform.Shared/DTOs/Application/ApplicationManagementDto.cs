using System.ComponentModel.DataAnnotations;

namespace AiDevPlatform.Shared.DTOs.Application;

// [联网-官方: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation?view=aspnetcore-8.0]
public sealed class SaveApplicationRequest
{
    [Required(ErrorMessage = "请输入应用名称。"), StringLength(200, ErrorMessage = "名称不能超过 200 个字符。")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "请输入应用编码。"), StringLength(50)]
    [RegularExpression("^[A-Za-z][A-Za-z0-9_-]{0,49}$", ErrorMessage = "编码须以字母开头，仅含字母、数字、下划线或短横线，最多 50 个字符。")]
    public string Code { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "描述不能超过 1000 个字符。")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "请输入版本。"), StringLength(20, ErrorMessage = "版本不能超过 20 个字符。")]
    public string Version { get; set; } = "1.0.0";
}

public sealed class ApplicationSummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Version { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}