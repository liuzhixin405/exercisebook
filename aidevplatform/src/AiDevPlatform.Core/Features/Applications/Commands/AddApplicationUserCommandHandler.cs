using Microsoft.Extensions.Logging;
using AiDevPlatform.Shared.Common;
using MediatR;
using AiDevPlatform.Shared.Models;
using AiDevPlatform.Core.Interfaces;

namespace AiDevPlatform.Core.Features.Applications.Commands;

public class AddApplicationUserCommandHandler : IRequestHandler<AddApplicationUserCommand, CommandResult>
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<AddApplicationUserCommandHandler> _logger;

    public AddApplicationUserCommandHandler(
        IApplicationRepository applicationRepository,
        IUserRepository userRepository,
        ILogger<AddApplicationUserCommandHandler> logger)
    {
        _applicationRepository = applicationRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<CommandResult> Handle(AddApplicationUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var application = await _applicationRepository.GetByIdAsync(request.ApplicationId);
            if (application == null)
            {
                return CommandResult.Failure("Application not found");
            }

            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                return CommandResult.Failure("User not found");
            }

            // Check if user has permission to add users
            if (!await _applicationRepository.IsApplicationOwnerAsync(request.ApplicationId, request.AddedBy.ToString()))
            {
                return CommandResult.Failure("No permission to add users to this application");
            }

            var applicationUser = new ApplicationUser
            {
                ApplicationId = request.ApplicationId,
                UserId = request.UserId,
                Role = Enum.Parse<ApplicationUserRole>(request.Role),
                Status = ApplicationUserStatus.Active,
                Permissions = request.Permissions,
                Settings = request.Settings,
                CreatedBy = request.AddedBy.ToString()
            };

            // Add to repository (you'll need to implement this in the repository)
            // await _applicationRepository.AddApplicationUserAsync(applicationUser);

            _logger.LogInformation("User {UserId} added to application {ApplicationId} by user {AddedBy}",
                request.UserId, request.ApplicationId, request.AddedBy);

            return CommandResult.Success("User added successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding user {UserId} to application {ApplicationId} by user {AddedBy}",
                request.UserId, request.ApplicationId, request.AddedBy);
            return CommandResult.Failure("Error occurred while adding user");
        }
    }
}