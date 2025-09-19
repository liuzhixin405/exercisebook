using Framework.Core.Abstractions.Visitors;
using Framework.Samples.Services;

namespace Framework.Samples.Visitors;

/// <summary>
/// 用户审计访问者
/// </summary>
public class UserAuditVisitor : IVisitor<User>
{
    /// <inheritdoc />
    public string Name => "UserAuditVisitor";

    /// <inheritdoc />
    public int Priority => 100;

    /// <inheritdoc />
    public async Task VisitAsync(User visitable)
    {
        Console.WriteLine($"审计用户: {visitable.Name} ({visitable.Email})");
        Console.WriteLine($"用户ID: {visitable.Id}");
        Console.WriteLine($"创建时间: {visitable.CreatedAt:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine($"更新时间: {visitable.UpdatedAt:yyyy-MM-dd HH:mm:ss}");
        
        // 模拟审计处理时间
        await Task.Delay(10);
        
        Console.WriteLine("用户审计完成");
    }

    /// <inheritdoc />
    public bool ShouldVisit(User visitable)
    {
        return visitable != null && visitable.UserId != Guid.Empty;
    }
}