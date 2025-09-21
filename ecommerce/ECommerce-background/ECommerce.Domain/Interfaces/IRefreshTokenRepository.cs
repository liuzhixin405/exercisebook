using ECommerce.Domain.Models;

namespace ECommerce.Domain.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task<RefreshToken> CreateAsync(RefreshToken refreshToken);
        Task<bool> RevokeAsync(string token);
        Task<bool> RevokeAllForUserAsync(Guid userId);
        Task<bool> DeleteExpiredAsync();
    }
}
