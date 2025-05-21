/*
 * 作者：feiwa/DESKTOP-CR8KUE4
 * 时间：2018-09-01 11:36:35
 * 版权：版权所有 (C) 小号科技 研发团队 2017~2018
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text; 
using Pandora.Cigfi.Models.Consts; 
namespace Pandora.Cigfi.Models.Sys
{

    [Table(TableNameConst.SYS_ADMINLOG)]
    /// <summary>管理日志表</summary>
    [Serializable]
    [Description("管理日志表")]
    public class Sys_AdminLogModel
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
        /// 管理员ID
        /// </summary>
        [DisplayName("管理员ID")]
        public Int32 UId { get; set; }

        /// <summary>          
        /// 唯一ID
        /// </summary>
        [DisplayName("唯一ID")]
        public String GUID { get; set; }

        /// <summary>          
        /// 用户名
        /// </summary>
        [DisplayName("用户名")]
        public String UserName { get; set; }
 

        /// <summary>          
        ///日志时间
        /// </summary>
        [DisplayName("日志时间")]
        public DateTime LogTime { get; set; }

        /// <summary>          
        /// 操作IP
        /// </summary>
        [DisplayName("操作IP")]
        public String LogIP { get; set; }
 

        /// <summary>          
        /// 日志记录
        /// </summary>
        [DisplayName("日志记录")]
        public String LogMsg { get; set; }

    

        /// <summary>          
        /// 日志类型
        /// </summary>
        [DisplayName("日志类型")]
        public string LogType { get; set; }
        /// <summary>          
        /// 日志级别，1：普通，2：告警，3：错误
        /// </summary>
        [DisplayName("日志级别")]
        public Int16 LogLevel { get; set; }
        
 

        #endregion


    }

    /// <summary>
    /// 管理员的日志级别
    /// </summary>
    public enum AdminLogLevel
    {
        /// <summary>
        /// 普通日志
        /// </summary>
     NORMAL = 1,
        /// <summary>
        /// 告警日志
        /// </summary>
     WARN = 2,
        /// <summary>
        /// 错误日志
        /// </summary>
         ERROR = 3,


    }

    /// <summary>
    /// 管理员的日志级别
    /// </summary>
    public enum AdminLogType
    {
        /// <summary>
        /// 登录成功
        /// </summary>
       LOGIN ,

        /// <summary>
        /// 登录失败
        /// </summary>
    LOGINFAIL,
        /// <summary>
        /// 添加操作
        /// </summary>
        ADD ,
        /// <summary>
        /// 修改操作
        /// </summary>
     EDIT ,
        /// <summary>
        /// 删除
        /// </summary>
        DEL ,
        /// <summary>
        /// 查看
        /// </summary>
        VIEW,
        /// <summary>
        /// 其他
        /// </summary>
       OTHER,
        /// <summary>
        /// 批量
        /// </summary>
        BATCH,
    }

}