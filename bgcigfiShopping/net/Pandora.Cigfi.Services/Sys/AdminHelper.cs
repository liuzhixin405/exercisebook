using Pandora.Cigfi.Models.Consts.Sys;
using FXH.Common.Data.Security;
using FXH.Web.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pandora.Cigfi.Services.Sys
{
   public  class AdminHelper
    {
        /// <summary>
        /// 当前登录用户的ID
        /// </summary>
        public static  string CurrentUserID
        {
            get
            {
                try
                {
                    string adminID = SessionHelper.GetSession(AdminKeyConsts.ADMINID).ToString();
                    //如果Session失效，则用Cookies判断
                    if (string.IsNullOrEmpty(adminID))
                    {
                        adminID = CookiesHelper.GetCookie(AdminKeyConsts.ADMINID);
                    }
                    if (!string.IsNullOrEmpty(adminID))
                    {
                        adminID = DESHelper.Decode(adminID, AdminKeyConsts.DESKEY);
                    }
                    return adminID;
                }
                catch(Exception ex)
                {
                    return "";
                }
              
            }
        }

        /// <summary>
        /// 当前登录用户的帐号
        /// </summary>
        public static string CurrentUserName
        {
            get
            {
                try
                {
                    string adminName = SessionHelper.GetSession(AdminKeyConsts.ADMINNAME).ToString();//ID 
                                                                                                     //如果Session失效，则用Cookies判断
                    if (string.IsNullOrEmpty(adminName))
                    {
                        adminName = CookiesHelper.GetCookie(AdminKeyConsts.ADMINNAME);
                    }
                    if (!string.IsNullOrEmpty(adminName))
                    {
                        adminName = DESHelper.Decode(adminName, AdminKeyConsts.DESKEY);
                    }
                   
                    return adminName;
                }
                catch (Exception ex)
                {
                    return "";
                }
            }
        }

    }
}
