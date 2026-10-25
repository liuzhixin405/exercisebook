using MediatR;
using AiDevPlatform.Shared.DTOs.User;

namespace AiDevPlatform.Core.Features.Auth.Queries;

public class GetCurrentUserQuery : IRequest<UserDto?>
{
    public Guid UserId { get; set; }
}
