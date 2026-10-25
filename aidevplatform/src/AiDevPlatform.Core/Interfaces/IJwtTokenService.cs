using AiDevPlatform.Shared.Models;

namespace AiDevPlatform.Core.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    bool ValidateToken(string token);
    string? GetUserIdFromToken(string token);
    string? GetUserNameFromToken(string token);
    string? GetUserEmailFromToken(string token);
    List<string> GetUserRolesFromToken(string token);
    List<string> GetUserPermissionsFromToken(string token);
    DateTime GetTokenExpiration(string token);
    bool IsTokenExpired(string token);
}
