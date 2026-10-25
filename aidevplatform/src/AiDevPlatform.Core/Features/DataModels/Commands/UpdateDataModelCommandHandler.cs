using MediatR;
using Microsoft.Extensions.Logging;
using AiDevPlatform.Core.Interfaces;
using AiDevPlatform.Shared.Common;
using AutoMapper;

namespace AiDevPlatform.Core.Features.DataModels.Commands;

public class UpdateDataModelCommandHandler : IRequestHandler<UpdateDataModelCommand, CommandResult>
{
    private readonly IDataModelRepository _dataModelRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateDataModelCommandHandler> _logger;

    public UpdateDataModelCommandHandler(
        IDataModelRepository dataModelRepository,
        IMapper mapper,
        ILogger<UpdateDataModelCommandHandler> logger)
    {
        _dataModelRepository = dataModelRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CommandResult> Handle(UpdateDataModelCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var dataModel = await _dataModelRepository.GetByIdAsync(request.Id);
            if (dataModel == null)
            {
                return CommandResult.FailureResult("数据模型不存在");
            }

            // Check if table name already exists (excluding current data model)
            if (await _dataModelRepository.TableNameExistsAsync(request.TableName, dataModel.ApplicationId, request.Id))
            {
                return CommandResult.FailureResult("表名已存在");
            }

            _mapper.Map(request, dataModel);
            dataModel.UpdatedBy = request.UpdatedBy.ToString();
            dataModel.UpdatedAt = DateTime.UtcNow;

            await _dataModelRepository.UpdateAsync(dataModel);

            return CommandResult.SuccessResult("数据模型更新成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating data model {DataModelId}", request.Id);
            return CommandResult.FailureResult("数据模型更新失败");
        }
    }
}
