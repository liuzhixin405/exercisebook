using ECommerce.Domain.Models;

namespace ECommerce.Domain.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        string GenerateRefreshToken();
        bool ValidateToken(string token);
        Guid? GetUserIdFromToken(string token);
        LoginResponseDto GenerateLoginResponse(User user);
    }
}
