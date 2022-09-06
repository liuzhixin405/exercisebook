using Abp.Application.Services;
using AbpVuenetcore.MultiTenancy.Dto;

namespace AbpVuenetcore.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}

