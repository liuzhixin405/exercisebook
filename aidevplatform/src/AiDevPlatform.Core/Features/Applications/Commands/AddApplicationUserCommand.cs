using MediatR;
using AiDevPlatform.Shared.Common;

namespace AiDevPlatform.Core.Features.Applications.Commands;

public class AddApplicationUserCommand : IRequest<CommandResult>
{
    public Guid ApplicationId { get; set; }
    public Guid UserId { get; set; }
    public string Role { get; set; } = string.Empty;
    public string? Permissions { get; set; }
    public string? Settings { get; set; }
    public Guid AddedBy { get; set; }
}
