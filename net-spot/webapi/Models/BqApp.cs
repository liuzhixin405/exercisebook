using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// APP配置表
    /// </summary>
    public partial class BqApp
    {
        public uint Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; } = null!;
        /// <summary>
        /// 平台
        /// </summary>
        public string Platform { get; set; } = null!;
        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { get; set; } = null!;
        /// <summary>
        /// IOS版下载次数
        /// </summary>
        public int IpaDownloadTimes { get; set; }
        /// <summary>
        /// Android版下载次数
        /// </summary>
        public int ApkDownloadTimes { get; set; }
        /// <summary>
        /// 版本更新时间
        /// </summary>
        public int UpdateTime { get; set; }
        /// <summary>
        /// 下载路径
        /// </summary>
        public string DownloadUrl { get; set; } = null!;
        /// <summary>
        /// 更新日志
        /// </summary>
        public string Note { get; set; } = null!;
        /// <summary>
        /// 下载次数
        /// </summary>
        public int DownloadTimes { get; set; }
        /// <summary>
        /// 最后一个强制更新的版本
        /// </summary>
        public string ForceVersion { get; set; } = null!;
    }
}
