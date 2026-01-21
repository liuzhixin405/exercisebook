using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using IdentityPaymentApi.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Logging;

namespace IdentityPaymentApi.Authorization;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<PermissionHandler> _logger;
    public PermissionHandler(ApplicationDbContext db, ILogger<PermissionHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        _logger.LogDebug("PermissionHandler invoked for requirement {Requirement}", requirement.PermissionName);

        // Try several claim types for user id (sub, nameidentifier, name)
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                     ?? context.User.FindFirst(ClaimTypes.Name)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogDebug("PermissionHandler: no user id found in claims");
            return;
        }

        var permissionName = requirement.PermissionName;

        var permission = await _db.Permissions.AsNoTracking().FirstOrDefaultAsync(p => p.Name == permissionName);
        if (permission == null)
        {
            _logger.LogDebug("PermissionHandler: permission {Permission} not found in DB", permissionName);
            return;
        }

        // 1) explicit user permission
        var up = await _db.UserPermissions.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == userId && u.PermissionId == permission.Id);
        if (up != null)
        {
            _logger.LogDebug("PermissionHandler: found explicit user permission for user {User} permission {Permission} IsAllowed={IsAllowed}", userId, permissionName, up.IsAllowed);
            if (up.IsAllowed) context.Succeed(requirement);
            return;
        }

        // 2) check roles from claims first
        var roleNames = context.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray();
        List<string> roleIds = new();
        if (roleNames.Length > 0)
        {
            roleIds = await _db.Roles.AsNoTracking().Where(r => roleNames.Contains(r.Name)).Select(r => r.Id).ToListAsync();
            _logger.LogDebug("PermissionHandler: mapped role names [{Roles}] to ids [{RoleIds}]", string.Join(',', roleNames), string.Join(',', roleIds));
        }

        // fallback: query AspNetUserRoles for role ids
        if (roleIds.Count == 0)
        {
            roleIds = await _db.UserRoles.AsNoTracking().Where(ur => ur.UserId == userId).Select(ur => ur.RoleId).ToListAsync();
            _logger.LogDebug("PermissionHandler: fetched role ids for user {User}: [{RoleIds}]", userId, string.Join(',', roleIds));
        }

        if (roleIds.Count == 0)
        {
            _logger.LogDebug("PermissionHandler: no roles found for user {User}", userId);
            return;
        }

        var rp = await _db.RolePermissions.AsNoTracking().AnyAsync(rp => roleIds.Contains(rp.RoleId) && rp.PermissionId == permission.Id);
        _logger.LogDebug("PermissionHandler: role permission lookup result for user {User} permission {Permission}: {Result}", userId, permissionName, rp);
        if (rp) context.Succeed(requirement);
    }
}
