using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 安全验证修改记录
    /// </summary>
    public partial class BqMemberModifyInformationRecord
    {
        public int Id { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public int FUserId { get; set; }
        /// <summary>
        /// 原联系方式
        /// </summary>
        public string FOldInformation { get; set; } = null!;
        /// <summary>
        /// 修改之后的联系方式
        /// </summary>
        public string FNewInformation { get; set; } = null!;
        /// <summary>
        /// 修改类型：1-手机，2-邮箱，3-谷歌
        /// </summary>
        public short FInformationType { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public int FModifyTime { get; set; }
        /// <summary>
        /// 修改ip地址
        /// </summary>
        public string FModifyIp { get; set; } = null!;
        /// <summary>
        /// 修改设备：1-PC，2-api
        /// </summary>
        public short FModifyServer { get; set; }
        /// <summary>
        /// 修改人
        /// </summary>
        public string FModifyUser { get; set; } = null!;
        /// <summary>
        /// 操作类型：1、注册；2、绑定；3、更改
        /// </summary>
        public short FOperationType { get; set; }
    }
}
