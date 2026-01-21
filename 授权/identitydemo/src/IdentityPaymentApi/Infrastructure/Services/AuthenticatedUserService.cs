using System.Security.Claims;
using IdentityPaymentApi.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace IdentityPaymentApi.Infrastructure.Services;

public class AuthenticatedUserService(IHttpContextAccessor httpContextAccessor) : IAuthenticatedUserService
{
    public string UserId { get; } = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
    public string UserName { get; } = httpContextAccessor.HttpContext?.User.Identity?.Name;
}
