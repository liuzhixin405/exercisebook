using Abp.AutoMapper;
using AbpVuenetcore.Authentication.External;

namespace AbpVuenetcore.Models.TokenAuth
{
    [AutoMapFrom(typeof(ExternalLoginProviderInfo))]
    public class ExternalLoginProviderInfoModel
    {
        public string Name { get; set; }

        public string ClientId { get; set; }
    }
}
