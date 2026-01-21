namespace IdentityPaymentApi.Application.Interfaces;

public interface IAuthenticatedUserService
{
    string UserId { get; }
    string UserName { get; }
}
