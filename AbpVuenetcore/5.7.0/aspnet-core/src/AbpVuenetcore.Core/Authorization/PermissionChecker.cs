using Abp.Authorization;
using AbpVuenetcore.Authorization.Roles;
using AbpVuenetcore.Authorization.Users;

namespace AbpVuenetcore.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
