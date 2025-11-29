using Framework.Infrastructure.Decorators;
using System.Text.Json;

namespace Framework.Samples.Decorators;

/// <summary>
/// 审计日志记录器实现 - 装饰器模式示例
/// </summary>
public class AuditLogger : IAuditLogger
{
    public Task LogAuditAsync(string operation, object? details = null)
    {
        var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
        Console.WriteLine($"[审计日志] {timestamp} - 操作: {operation}");
        if (details != null)
        {
            var json = JsonSerializer.Serialize(details, new JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine($"[审计日志] 详情: {json}");
        }
        return Task.CompletedTask;
    }
}
