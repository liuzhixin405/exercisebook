using MediatR;
using Microsoft.Extensions.Logging;
using AutoMapper;
using AiDevPlatform.Core.Interfaces;
using AiDevPlatform.Shared.Common;
using AiDevPlatform.Shared.DTOs.ApplicationPage;
using AiDevPlatform.Shared.Models;

namespace AiDevPlatform.Core.Features.ApplicationPages.Commands;

public class CreateApplicationPageCommandHandler : IRequestHandler<CreateApplicationPageCommand, CommandResult<ApplicationPageDto>>
{
    private readonly IApplicationPageRepository _applicationPageRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateApplicationPageCommandHandler> _logger;

    public CreateApplicationPageCommandHandler(
        IApplicationPageRepository applicationPageRepository,
        IMapper mapper,
        ILogger<CreateApplicationPageCommandHandler> logger)
    {
        _applicationPageRepository = applicationPageRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CommandResult<ApplicationPageDto>> Handle(CreateApplicationPageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if route already exists
            if (await _applicationPageRepository.RouteExistsAsync(request.ApplicationId, request.Route))
            {
                return CommandResult.Failure<ApplicationPageDto>("页面路由已存在");
            }

            var applicationPage = new ApplicationPage
            {
                Name = request.Name,
                Description = request.Description,
                Route = request.Route,
                Type = request.Type,
                Layout = request.Layout,
                Components = request.Components,
                Styles = request.Styles,
                Scripts = request.Scripts,
                DataBinding = request.DataBinding,
                IsActive = request.IsActive,
                IsPublic = request.IsPublic,
                SortOrder = request.SortOrder,
                ApplicationId = request.ApplicationId,
                ParentPageId = request.ParentPageId,
                CreatedBy = request.CreatedBy,
                CreatedAt = DateTime.UtcNow
            };

            await _applicationPageRepository.AddAsync(applicationPage);

            var applicationPageDto = _mapper.Map<ApplicationPageDto>(applicationPage);
            return CommandResult.Success(applicationPageDto, "页面创建成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating application page {PageName}", request.Name);
            return CommandResult.Failure<ApplicationPageDto>("页面创建失败");
        }
    }
}
