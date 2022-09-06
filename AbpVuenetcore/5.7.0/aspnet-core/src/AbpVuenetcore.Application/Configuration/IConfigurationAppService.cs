using System.Threading.Tasks;
using AbpVuenetcore.Configuration.Dto;

namespace AbpVuenetcore.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
