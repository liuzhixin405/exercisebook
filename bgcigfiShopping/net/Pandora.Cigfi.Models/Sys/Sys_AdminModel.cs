/*
 * 作者：feiwa/DESKTOP-CR8KUE4
 * 时间：2018-09-01 11:21:25
 * 版权：版权所有 (C) 小号科技 研发团队 2017~2018
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 
using Pandora.Cigfi.Models.Consts;

namespace Pandora.Cigfi.Models.Sys
{

    [Table(TableNameConst.SYS_ADMIN)]
    /// <summary>管理员</summary>
    [Serializable]
    [Description("管理员")]
    public class Sys_AdminModel
    {
        #region 属性
        [Dapper.Contrib.Extensions.Key] 
        //自动增长
        /// <summary>          
         /// 编号
         /// </summary>
        [DisplayName("编号")]
        public Int32 Id { get; set; }

        /// <summary>          
        /// 用户名
        /// </summary>
        [DisplayName("用户名")]
        public String UserName { get; set; }

        /// <summary>          
        /// 密码
        /// </summary>
        [DisplayName("密码")]
        public String PassWord { get; set; }

        /// <summary>          
        /// 盐值
        /// </summary>
        [DisplayName("盐值")]
        public String Salt { get; set; }

        /// <summary>          
        /// 姓名
        /// </summary>
        [DisplayName("姓名")]
        public String RealName { get; set; }

        /// <summary>          
        /// 电话
        /// </summary>
        [DisplayName("电话")]
        public String Tel { get; set; }

        /// <summary>          
        /// 邮件
        /// </summary>
        [DisplayName("邮件")]
        public String Email { get; set; }

        /// <summary>          
        /// 级别
        /// </summary>
        [DisplayName("级别")]
        public Int32 UserLevel { get; set; }

        /// <summary>          
        /// 管理组
        /// </summary>
        [DisplayName("管理组")]
        public Int32 RoleId { get; set; }

        /// <summary>          
        /// 用户组
        /// </summary>
        [DisplayName("用户组")]
        public Int32 GroupId { get; set; }

        /// <summary>          
        /// 最后登录时间
        /// </summary>
        [DisplayName("最后登录时间")]
        public DateTime LastLoginTime { get; set; }

        /// <summary>          
        /// 上次登录IP
        /// </summary>
        [DisplayName("上次登录IP")]
        public String LastLoginIP { get; set; }

        /// <summary>          
        /// 本次登录时间
        /// </summary>
        [DisplayName("本次登录时间")]
        public DateTime ThisLoginTime { get; set; }

        /// <summary>          
        /// 本次登录IP
        /// </summary>
        [DisplayName("本次登录IP")]
        public String ThisLoginIP { get; set; }

        /// <summary>          
        /// 是否是锁定
        /// </summary>
        [DisplayName("是否是锁定")]
        public Int32 IsLock { get; set; }

        #endregion


    }

    public class Sys_AdminViewModel: Sys_AdminModel
    {
        public string RoleName
        {
            get;
            set;
        }
    }
}