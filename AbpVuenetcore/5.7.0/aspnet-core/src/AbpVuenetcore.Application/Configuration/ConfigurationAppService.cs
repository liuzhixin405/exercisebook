using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using AbpVuenetcore.Configuration.Dto;

namespace AbpVuenetcore.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : AbpVuenetcoreAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
