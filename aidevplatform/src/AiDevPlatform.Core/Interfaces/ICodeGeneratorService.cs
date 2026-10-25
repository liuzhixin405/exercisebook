using AiDevPlatform.Shared.Models;

namespace AiDevPlatform.Core.Interfaces;

public interface ICodeGeneratorService
{
    Task<string> GenerateApplicationCodeAsync(Application application);
    Task<string> GenerateDataModelCodeAsync(DataModel dataModel);
    Task<string> GenerateFormCodeAsync(Form form);
    Task<string> GenerateWorkflowCodeAsync(Workflow workflow);
    Task<string> GenerateReportCodeAsync(Report report);
    Task<string> GenerateComponentCodeAsync(Component component);
    Task<string> GenerateApiControllerCodeAsync(DataModel dataModel);
    Task<string> GenerateServiceCodeAsync(DataModel dataModel);
    Task<string> GenerateRepositoryCodeAsync(DataModel dataModel);
    Task<string> GenerateDtoCodeAsync(DataModel dataModel);
    Task<string> GenerateValidationCodeAsync(DataModel dataModel);
    Task<string> GenerateMigrationCodeAsync(DataModel dataModel);
    Task<string> GenerateSeedDataCodeAsync(DataModel dataModel);
    Task<string> GenerateTestCodeAsync(DataModel dataModel);
    Task<string> GenerateDocumentationCodeAsync(DataModel dataModel);
}
