using MediatR;
using Microsoft.Extensions.Logging;
using AiDevPlatform.Core.Interfaces;
using AiDevPlatform.Shared.Common;
using AiDevPlatform.Shared.Models;
using AutoMapper;

namespace AiDevPlatform.Core.Features.DataModels.Commands;

public class CreateDataModelCommandHandler : IRequestHandler<CreateDataModelCommand, CommandResult>
{
    private readonly IDataModelRepository _dataModelRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateDataModelCommandHandler> _logger;

    public CreateDataModelCommandHandler(
        IDataModelRepository dataModelRepository,
        IMapper mapper,
        ILogger<CreateDataModelCommandHandler> logger)
    {
        _dataModelRepository = dataModelRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CommandResult> Handle(CreateDataModelCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if table name already exists
            if (await _dataModelRepository.TableNameExistsAsync(request.TableName, request.ApplicationId))
            {
                return CommandResult.FailureResult("表名已存在");
            }

            var dataModel = _mapper.Map<DataModel>(request);
            dataModel.CreatedBy = request.CreatedBy.ToString();
            dataModel.CreatedAt = DateTime.UtcNow;

            await _dataModelRepository.AddAsync(dataModel);

            return CommandResult.SuccessResult("数据模型创建成功", dataModel.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating data model {DataModelName}", request.Name);
            return CommandResult.FailureResult("数据模型创建失败");
        }
    }
}
