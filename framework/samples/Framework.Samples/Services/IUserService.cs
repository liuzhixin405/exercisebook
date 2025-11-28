using Framework.Core.Abstractions.Visitors;

namespace Framework.Samples.Services;

/// <summary>
/// 用户服务接口
/// </summary>
public interface IUserService
{
    /// <summary>
    /// 创建用户
    /// </summary>
    /// <param name="userName">用户名</param>
    /// <param name="email">邮箱</param>
    /// <param name="password">密码</param>
    /// <returns>用户ID</returns>
    Task<Guid> CreateUserAsync(string userName, string email, string password);

    /// <summary>
    /// 更新用户
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="userName">用户名</param>
    /// <param name="email">邮箱</param>
    /// <returns>是否更新成功</returns>
    Task<bool> UpdateUserAsync(Guid userId, string userName, string email);

    /// <summary>
    /// 获取用户
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>用户信息</returns>
    Task<User?> GetUserAsync(Guid userId);

    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>是否删除成功</returns>
    Task<bool> DeleteUserAsync(Guid userId);
}

/// <summary>
/// 用户信息
/// </summary>
public class User : IVisitable
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 邮箱
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // IVisitable implementation

    string IVisitable.Type => "User";

    // Explicit implementation to keep Guid Id while satisfying IVisitable.Id (string)
    string IVisitable.Id => Id.ToString();

    public async Task AcceptAsync(IVisitor<IVisitable> visitor)
    {
        if (visitor is IVisitor<User> typedVisitor)
        {
            await typedVisitor.VisitAsync(this);
            return;
        }

        var visitMethod = visitor.GetType().GetMethod("VisitAsync");
        if (visitMethod != null)
        {
            var task = (Task?)visitMethod.Invoke(visitor, new object[] { this });
            if (task != null)
                await task;
        }
    }
}
