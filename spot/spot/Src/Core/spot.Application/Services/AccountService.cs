using Microsoft.Extensions.Options;
using spot.Application.DTOs.Account.Requests;
using spot.Application.DTOs.Account.Responses;
using spot.Application.Interfaces.UserInterfaces;
using spot.Application.Settings;
using spot.Application.Wrappers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace spot.Application.Services
{
    public class AccountService : IAccountServices
    {
        private readonly TokenService _tokenService;

        public AccountService(IOptions<JWTSettings> jwtSettings)
        {
            _tokenService = new TokenService(jwtSettings);
        }

        public Task<BaseResult<AuthenticationResponse>> Authenticate(AuthenticationRequest request)
        {
            // 这里简化处理，仅作演示用途
            // 在实际应用中，应该查询数据库验证用户凭据
            
            // 模拟验证通过情况
            if (request.UserName == "admin" && request.Password == "P@ssw0rd")
            {
                var user = new AuthenticationResponse
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = request.UserName,
                    Email = "admin@example.com",
                    Roles = new List<string> { "Administrator" },
                    IsVerified = true
                };

                // 生成JWT令牌
                user.JwToken = _tokenService.GenerateJwtToken(user);

                return Task.FromResult(BaseResult<AuthenticationResponse>.Ok(user));
            }

            // 验证失败
            return Task.FromResult(BaseResult<AuthenticationResponse>.Failure(new Error(ErrorCode.ModelStateNotValid, "Invalid username or password", "Authentication")));
        }

        public Task<BaseResult<AuthenticationResponse>> AuthenticateByUserName(string username)
        {
            // 简化实现，仅做演示
            var user = new AuthenticationResponse
            {
                Id = Guid.NewGuid().ToString(),
                UserName = username,
                Email = $"{username}@example.com",
                Roles = new List<string> { "User" },
                IsVerified = true
            };

            // 生成JWT令牌
            user.JwToken = _tokenService.GenerateJwtToken(user);

            return Task.FromResult(BaseResult<AuthenticationResponse>.Ok(user));
        }

        public Task<BaseResult> ChangePassword(ChangePasswordRequest model)
        {
            // 简化实现，实际应用中需要验证用户身份并更新密码
            return Task.FromResult(BaseResult.Ok());
        }

        public Task<BaseResult> ChangeUserName(ChangeUserNameRequest model)
        {
            // 简化实现，实际应用中需要验证用户身份并更新用户名
            return Task.FromResult(BaseResult.Ok());
        }

        public Task<BaseResult<string>> RegisterGhostAccount()
        {
            // 简化实现，实际应用中需要创建新用户记录
            var userId = Guid.NewGuid().ToString();
            return Task.FromResult(BaseResult<string>.Ok(userId));
        }
    }
} 