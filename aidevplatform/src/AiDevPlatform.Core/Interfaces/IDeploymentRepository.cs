using AiDevPlatform.Shared.Models;
using AiDevPlatform.Shared.DTOs.Common;

namespace AiDevPlatform.Core.Interfaces;

public interface IDeploymentRepository : IRepository<Deployment>
{
    Task<PagedResult<Deployment>> GetPagedDeploymentsAsync(PagedRequest request, Guid applicationId);
    Task<IEnumerable<Deployment>> GetApplicationDeploymentsAsync(Guid applicationId);
    Task<IEnumerable<Deployment>> GetDeploymentsByStatusAsync(DeploymentStatus status);
    Task<Deployment?> GetLatestDeploymentAsync(Guid applicationId, string? environment = null);
    Task UpdateDeploymentStatusAsync(Guid deploymentId, DeploymentStatus status);
    Task AddDeploymentLogAsync(Guid deploymentId, string level, string message, string? details = null);
}
