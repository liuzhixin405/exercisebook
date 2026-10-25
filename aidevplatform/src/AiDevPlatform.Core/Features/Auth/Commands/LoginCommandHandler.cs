using Microsoft.Extensions.Logging;
using MediatR;
using Microsoft.AspNetCore.Identity;
using AiDevPlatform.Shared.Models;
using AiDevPlatform.Shared.DTOs.User;
using AiDevPlatform.Shared.DTOs.Auth;
using AiDevPlatform.Core.Interfaces;

namespace AiDevPlatform.Core.Features.Auth.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResultDto>
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        UserManager<User> userManager,
        IJwtTokenService jwtTokenService,
        IUserRepository userRepository,
        ILogger<LoginCommandHandler> logger)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<LoginResultDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
            {
                return new LoginResultDto
                {
                    Success = false,
                    Message = "用户名或密码错误"
                };
            }

            if (!user.IsActive)
            {
                return new LoginResultDto
                {
                    Success = false,
                    Message = "账户已被禁用"
                };
            }

            var result = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!result)
            {
                return new LoginResultDto
                {
                    Success = false,
                    Message = "用户名或密码错误"
                };
            }

            // Update last login time
            user.LastLoginAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            // Generate tokens
            var accessToken = _jwtTokenService.GenerateAccessToken(user);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();

            // Get user roles
            var roles = await _userManager.GetRolesAsync(user);

            var userDto = new UserDto
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Avatar = user.Avatar,
                LastLoginAt = user.LastLoginAt,
                IsActive = user.IsActive,
                TimeZone = user.TimeZone,
                Language = user.Language,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                Roles = roles.ToList()
            };

            return new LoginResultDto
            {
                Success = true,
                Token = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = _jwtTokenService.GetTokenExpiration(accessToken),
                User = userDto,
                Message = "登录成功"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user {UserName}", request.UserName);
            return new LoginResultDto
            {
                Success = false,
                Message = "登录过程中发生错误"
            };
        }
    }
}
