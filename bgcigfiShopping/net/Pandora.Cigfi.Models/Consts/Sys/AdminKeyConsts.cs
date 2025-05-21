using System;
using System.Collections.Generic;
using System.Text;

namespace Pandora.Cigfi.Models.Consts.Sys
{
   public  class AdminKeyConsts
    {
        private const string prefixKey = "cms_";

        /// <summary>
        /// des加密密钥
        /// </summary>
        public const string DESKEY = "fxh_des_2019_key";
        
        #region 静态key
        /// <summary>
        /// Session 或Cookie 中后台管理员帐号
        /// </summary>
        public static string ADMINNAME = prefixKey + "AdminName";

        /// <summary>
        /// Session 或Cookie 中后台管理员ID
        /// </summary>
        public static string ADMINID = prefixKey + "AdminID";

        /// <summary>
        /// Session 或Cookie 中后台管理员是否是超级管理员Key
        /// </summary>
        public static string ISSUPPERADMIN = prefixKey + "IsSupperAdmin";

        /// <summary>
        /// Session 或Cookie 中管理员权限
        /// </summary>
        public static string ADMINPOWER = prefixKey + "AdminPower";

        /// <summary>
        /// Session 或Cookie 中管理员后台日志ID，本次日志Key
        /// </summary>
        public static string ADMINLOGID = prefixKey + "AdminLogID";


        /// <summary>
        /// Session 或 cookie 中后台管理员信息，加密信息
        /// </summary>
        public static string ADMININFO = prefixKey + "AdminInfo";

        /// <summary>
        /// Session 或 cookie 中后台管理员登陆初始时间
        /// </summary>
        public static string AdminLoginTime = prefixKey + "AdminLoginTime";


        #endregion
    }
}
