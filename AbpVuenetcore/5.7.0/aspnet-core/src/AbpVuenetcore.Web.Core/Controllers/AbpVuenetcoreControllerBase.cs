using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace AbpVuenetcore.Controllers
{
    public abstract class AbpVuenetcoreControllerBase: AbpController
    {
        protected AbpVuenetcoreControllerBase()
        {
            LocalizationSourceName = AbpVuenetcoreConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
