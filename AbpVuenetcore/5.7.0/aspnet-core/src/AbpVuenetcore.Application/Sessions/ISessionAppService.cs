using System.Threading.Tasks;
using Abp.Application.Services;
using AbpVuenetcore.Sessions.Dto;

namespace AbpVuenetcore.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
