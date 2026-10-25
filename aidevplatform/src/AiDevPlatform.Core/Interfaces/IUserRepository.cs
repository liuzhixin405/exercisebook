using AiDevPlatform.Shared.Models;
using AiDevPlatform.Shared.DTOs.Common;

namespace AiDevPlatform.Core.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUserNameAsync(string userName);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetUserWithRolesAsync(Guid id);
    Task<User?> GetUserWithPermissionsAsync(Guid id);
    Task<PagedResult<User>> GetPagedUsersAsync(PagedRequest request);
    Task<IEnumerable<User>> GetUsersByRoleAsync(string role);
    Task<IEnumerable<User>> GetUsersByOrganizationAsync(Guid organizationId);
    Task<bool> UserNameExistsAsync(string userName, Guid? excludeId = null);
    Task<bool> EmailExistsAsync(string email, Guid? excludeId = null);
    Task UpdateLastLoginAsync(Guid userId);
}
