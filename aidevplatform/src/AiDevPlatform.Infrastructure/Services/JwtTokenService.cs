using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using AiDevPlatform.Shared.Models;
using AiDevPlatform.Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace AiDevPlatform.Infrastructure.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<JwtTokenService> _logger;

    public JwtTokenService(IConfiguration configuration, ILogger<JwtTokenService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public string GenerateAccessToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured"));
        var expiryMinutes = int.Parse(_configuration["Jwt:ExpiryMinutes"] ?? "60");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new("sub", user.Id.ToString()),
            new("name", user.UserName ?? string.Empty),
            new("email", user.Email ?? string.Empty),
            new("firstname", user.FirstName),
            new("lastname", user.LastName),
            new("avatar", user.Avatar ?? string.Empty),
            new("timezone", user.TimeZone ?? string.Empty),
            new("language", user.Language ?? "zh-CN")
        };

        // Add roles and permissions (you would typically get these from the database)
        // For now, we'll add a default role
        claims.Add(new Claim(ClaimTypes.Role, "User"));
        claims.Add(new Claim("role", "User"));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public bool ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured"));

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Token validation failed");
            return false;
        }
    }

    public string? GetUserIdFromToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            return jwtToken.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get user ID from token");
            return null;
        }
    }

    public string? GetUserNameFromToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            return jwtToken.Claims.FirstOrDefault(x => x.Type == "name")?.Value;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get username from token");
            return null;
        }
    }

    public string? GetUserEmailFromToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            return jwtToken.Claims.FirstOrDefault(x => x.Type == "email")?.Value;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get email from token");
            return null;
        }
    }

    public List<string> GetUserRolesFromToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            return jwtToken.Claims.Where(x => x.Type == "role").Select(x => x.Value).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get roles from token");
            return new List<string>();
        }
    }

    public List<string> GetUserPermissionsFromToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            return jwtToken.Claims.Where(x => x.Type == "permission").Select(x => x.Value).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get permissions from token");
            return new List<string>();
        }
    }

    public DateTime GetTokenExpiration(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            return jwtToken.ValidTo;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get token expiration");
            return DateTime.MinValue;
        }
    }

    public bool IsTokenExpired(string token)
    {
        try
        {
            var expiration = GetTokenExpiration(token);
            return expiration < DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to check token expiration");
            return true;
        }
    }
}
