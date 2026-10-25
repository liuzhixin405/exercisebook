using System.ComponentModel.DataAnnotations;
using AiDevPlatform.Core.Interfaces;
using AiDevPlatform.Shared.DTOs.Application;
using AiDevPlatform.Shared.Models;

namespace AiDevPlatform.Core.Features.Applications;

public sealed class ApplicationManagementService
{
    private static readonly Guid DevelopmentOwnerId = new("11111111-1111-4111-8111-111111111111");
    private static readonly SemaphoreSlim WriteLock = new(1, 1);
    private readonly IApplicationRepository _applications;
    private readonly IUserRepository _users;

    public ApplicationManagementService(IApplicationRepository applications, IUserRepository users)
    {
        _applications = applications;
        _users = users;
    }

    public async Task<List<ApplicationSummaryDto>> ListAsync()
    {
        var applications = await _applications.FindAsync(application => application.OwnerId == DevelopmentOwnerId);
        return applications.OrderByDescending(application => application.CreatedAt).Select(ToDto).ToList();
    }

    public async Task<ApplicationSummaryDto?> GetAsync(Guid id)
    {
        var application = await FindAsync(id);
        return application == null ? null : ToDto(application);
    }

    public async Task<ApplicationSummaryDto?> SaveAsync(Guid? id, SaveApplicationRequest request,
        CancellationToken cancellationToken)
    {
        // [联网-官方: https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.validator.validateobject?view=net-8.0]
        Validator.ValidateObject(request, new ValidationContext(request), true);
        var code = request.Code.ToUpperInvariant();

        // [联网-官方: https://learn.microsoft.com/en-us/dotnet/api/system.threading.semaphoreslim.waitasync?view=net-8.0]
        await WriteLock.WaitAsync(cancellationToken);
        try
        {
            var application = id.HasValue ? await FindAsync(id.Value) : new Application();
            if (application == null)
                return null;
            if (await _applications.CodeExistsAsync(code, id))
                throw new InvalidOperationException("应用编码已存在，请使用其他编码。");

            if (!id.HasValue)
            {
                var owner = await _users.GetByIdAsync(DevelopmentOwnerId);
                if (owner == null)
                    await _users.AddAsync(new User
                    {
                        Id = DevelopmentOwnerId,
                        UserName = "local-development",
                        NormalizedUserName = "LOCAL-DEVELOPMENT",
                        Email = "local-development@example.invalid",
                        NormalizedEmail = "LOCAL-DEVELOPMENT@EXAMPLE.INVALID",
                        FirstName = "本机开发",
                        SecurityStamp = Guid.NewGuid().ToString()
                    });
                application.OwnerId = DevelopmentOwnerId;
                application.CreatedBy = DevelopmentOwnerId.ToString();
            }

            application.Name = request.Name.Trim();
            application.Code = code;
            application.Description = request.Description?.Trim();
            application.Version = request.Version.Trim();
            application.UpdatedBy = DevelopmentOwnerId.ToString();
            if (id.HasValue)
                await _applications.UpdateAsync(application);
            else
                await _applications.AddAsync(application);
            return ToDto(application);
        }
        finally
        {
            WriteLock.Release();
        }
    }

    private Task<Application?> FindAsync(Guid id) => _applications.FirstOrDefaultAsync(
        application => application.Id == id && application.OwnerId == DevelopmentOwnerId);

    private static ApplicationSummaryDto ToDto(Application application) => new()
    {
        Id = application.Id,
        Name = application.Name,
        Code = application.Code,
        Description = application.Description,
        Version = application.Version,
        CreatedAt = application.CreatedAt,
        UpdatedAt = application.UpdatedAt
    };
}