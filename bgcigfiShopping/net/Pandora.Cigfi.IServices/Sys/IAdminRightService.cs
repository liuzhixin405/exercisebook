using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pandora.Cigfi.IServices
{
    /// <summary>
    /// 管理员权限接口
    /// </summary>
    public interface IAdminRightService
    {
        /// <summary>
        /// 检查管理员有没有管理权限
        /// </summary>
        /// <param name="eventKey">动作，如add、edit</param>
        /// <param name="menuKey">菜单名称，如article、product</param>
        /// <returns></returns>
        Task<bool> CheckAdminPower(string eventKey, string menuKey);

    }
}
