using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Models;
using BCrypt.Net;

namespace ECommerce.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IJwtService _jwtService;

        public AuthService(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IJwtService jwtService)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _jwtService = jwtService;
        }

        public async Task<LoginResponseDto?> RegisterAsync(CreateUserDto createUserDto)
        {
            // Check if user already exists
            if (await _userRepository.ExistsByEmailAsync(createUserDto.Email))
                throw new InvalidOperationException("User with this email already exists");

            // Create new user
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = createUserDto.UserName,
                Email = createUserDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password),
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                PhoneNumber = createUserDto.PhoneNumber,
                Address = createUserDto.Address,
                Role = createUserDto.Role ?? "User",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);

            // Generate login response
            var loginResponse = _jwtService.GenerateLoginResponse(user);

            // Save refresh token
            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = loginResponse.RefreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            await _refreshTokenRepository.CreateAsync(refreshToken);
          
            return loginResponse;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);
            if (user == null || !user.IsActive)
                return null;

            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                return null;

            // Update last login time
            user.LastLoginAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);

            // Generate login response
            var loginResponse = _jwtService.GenerateLoginResponse(user);

            // Save refresh token
            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = loginResponse.RefreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            await _refreshTokenRepository.CreateAsync(refreshToken);

            return loginResponse;
        }

        public async Task<LoginResponseDto?> RefreshTokenAsync(string refreshToken)
        {
            var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
            if (storedToken == null || storedToken.ExpiresAt < DateTime.UtcNow)
                return null;

            var user = await _userRepository.GetByIdAsync(storedToken.UserId);
            if (user == null || !user.IsActive)
                return null;

            // Revoke the old refresh token
            await _refreshTokenRepository.RevokeAsync(refreshToken);

            // Generate new tokens
            var loginResponse = _jwtService.GenerateLoginResponse(user);

            // Save new refresh token
            var newRefreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = loginResponse.RefreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            await _refreshTokenRepository.CreateAsync(newRefreshToken);

            return loginResponse;
        }

        public async Task<bool> LogoutAsync(string refreshToken)
        {
            return await _refreshTokenRepository.RevokeAsync(refreshToken);
        }

        public async Task<bool> LogoutAllAsync(Guid userId)
        {
            return await _refreshTokenRepository.RevokeAllForUserAsync(userId);
        }
    }
}
