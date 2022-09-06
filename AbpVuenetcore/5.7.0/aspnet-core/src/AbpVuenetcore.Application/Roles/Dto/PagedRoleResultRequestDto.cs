using Abp.Application.Services.Dto;

namespace AbpVuenetcore.Roles.Dto
{
    public class PagedRoleResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}

