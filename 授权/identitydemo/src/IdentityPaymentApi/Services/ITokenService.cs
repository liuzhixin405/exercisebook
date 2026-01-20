using IdentityPaymentApi.Models;

namespace IdentityPaymentApi.Services;

public interface ITokenService
{
    Task<string> CreateTokenAsync(ApplicationUser user);
}