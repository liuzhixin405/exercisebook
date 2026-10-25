using MediatR;
using AiDevPlatform.Shared.DTOs.User;
using AiDevPlatform.Shared.DTOs.Auth;

namespace AiDevPlatform.Core.Features.Auth.Commands;

public class LoginCommand : IRequest<LoginResultDto>
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; }
}
