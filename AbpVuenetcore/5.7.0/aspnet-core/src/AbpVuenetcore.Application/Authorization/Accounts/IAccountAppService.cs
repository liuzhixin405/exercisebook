using System.Threading.Tasks;
using Abp.Application.Services;
using AbpVuenetcore.Authorization.Accounts.Dto;

namespace AbpVuenetcore.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
