using AiDevPlatform.Shared.Models;
using AiDevPlatform.Shared.DTOs.Common;

namespace AiDevPlatform.Core.Interfaces;

public interface IDataModelRepository : IRepository<DataModel>
{
    Task<PagedResult<DataModel>> GetPagedDataModelsAsync(PagedRequest request, Guid applicationId);
    Task<DataModel?> GetDataModelWithFieldsAsync(Guid id);
    Task<DataModel?> GetDataModelWithRelationshipsAsync(Guid id);
    Task<IEnumerable<DataModel>> GetApplicationDataModelsAsync(Guid applicationId);
    Task<DataModel?> GetByTableNameAsync(string tableName, string? schema = null);
    Task<bool> TableNameExistsAsync(string tableName, Guid applicationId, Guid? excludeId = null);
    Task<IEnumerable<DataModelField>> GetDataModelFieldsAsync(Guid dataModelId);
    Task<IEnumerable<DataModelRelationship>> GetDataModelRelationshipsAsync(Guid dataModelId);
    Task<bool> ColumnNameExistsAsync(string columnName, Guid dataModelId, Guid? excludeId = null);
    Task<DataModelField?> GetFieldByIdAsync(Guid fieldId);
    Task<DataModelRelationship?> GetRelationshipByIdAsync(Guid relationshipId);
    Task AddFieldAsync(DataModelField field);
    Task AddRelationshipAsync(DataModelRelationship relationship);
    Task UpdateFieldAsync(DataModelField field);
    Task UpdateRelationshipAsync(DataModelRelationship relationship);
}
