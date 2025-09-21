using ECommerce.Domain.Models;

namespace ECommerce.Domain.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> RegisterAsync(CreateUserDto createUserDto);
        Task<LoginResponseDto?> LoginAsync(LoginDto loginDto);
        Task<LoginResponseDto?> RefreshTokenAsync(string refreshToken);
        Task<bool> LogoutAsync(string refreshToken);
        Task<bool> LogoutAllAsync(Guid userId);
    }
}
