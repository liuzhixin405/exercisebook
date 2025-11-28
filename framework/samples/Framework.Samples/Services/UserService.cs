using Framework.Core.Abstractions.Events;
using Framework.Samples.Events;

namespace Framework.Samples.Services;

/// <summary>
/// 用户服务实现
/// </summary>
public class UserService : IUserService
{
    private readonly IEventBus _eventBus;
    private readonly Dictionary<Guid, User> _users;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="eventBus">事件总线</param>
    public UserService(IEventBus eventBus)
    {
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        _users = new Dictionary<Guid, User>();
    }

    /// <inheritdoc />
    public async Task<Guid> CreateUserAsync(string userName, string email, string password)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = userName,
            Email = email,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _users[user.Id] = user;

        // 发布用户创建事件
        var userCreatedEvent = new UserCreatedEvent
        {
            UserId = user.Id,
            UserName = user.Name,
            Email = user.Email
        };

        await _eventBus.PublishAsync(userCreatedEvent);

        Console.WriteLine($"用户创建成功: {user.Name} ({user.Email})");
        return user.Id;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateUserAsync(Guid userId, string userName, string email)
    {
        if (!_users.TryGetValue(userId, out var user))
        {
            return false;
        }

        var oldName = user.Name;
        var oldEmail = user.Email;

        user.Name = userName;
        user.Email = email;
        user.UpdatedAt = DateTime.UtcNow;

        // 发布用户更新事件
        var userUpdatedEvent = new UserUpdatedEvent
        {
            UserId = user.Id,
            OldName = oldName,
            NewName = user.Name,
            OldEmail = oldEmail,
            NewEmail = user.Email
        };

        await _eventBus.PublishAsync(userUpdatedEvent);

        Console.WriteLine($"用户更新成功: {user.Name} ({user.Email})");
        return true;
    }

    /// <inheritdoc />
    public Task<User?> GetUserAsync(Guid userId)
    {
        _users.TryGetValue(userId, out var user);
        return Task.FromResult(user);
    }

    /// <inheritdoc />
    public Task<bool> DeleteUserAsync(Guid userId)
    {
        var removed = _users.Remove(userId);
        if (removed)
        {
            Console.WriteLine($"用户删除成功: {userId}");
        }
        return Task.FromResult(removed);
    }
}
