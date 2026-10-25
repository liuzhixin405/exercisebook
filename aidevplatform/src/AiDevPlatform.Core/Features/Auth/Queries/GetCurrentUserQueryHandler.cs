using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;
using AiDevPlatform.Core.Interfaces;
using AiDevPlatform.Shared.DTOs.User;
using Microsoft.AspNetCore.Identity;
using AiDevPlatform.Shared.Models;

namespace AiDevPlatform.Core.Features.Auth.Queries;

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserDto?>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetCurrentUserQueryHandler> _logger;
    private readonly UserManager<User> _userManager;

    public GetCurrentUserQueryHandler(IUserRepository userRepository, IMapper mapper, ILogger<GetCurrentUserQueryHandler> logger, UserManager<User> userManager)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
        _userManager = userManager;
    }

    public async Task<UserDto?> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                return null;
            }

            var userDto = _mapper.Map<UserDto>(user);
            userDto.Roles = (await _userManager.GetRolesAsync(user)).ToList();
            // TODO: Add permissions if needed

            return userDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current user {UserId}", request.UserId);
            throw;
        }
    }
}
