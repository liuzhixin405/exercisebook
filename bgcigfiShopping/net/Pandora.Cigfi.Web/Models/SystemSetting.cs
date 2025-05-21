﻿using  System;
using  System.Collections.Generic;
using   System.Linq;
using  System.Threading.Tasks;

namespace Pandora.Cigfi.Web.Models
{
    /// <summary>
    /// 系统部分配置
    /// </summary>
    public class SystemSetting
    {
        /// <summary>
        /// 文件允许扩展名
        /// </summary>
        public string FileAllowedExtensions { get; set; }
        /// <summary>
        /// 图片允许扩展名
        /// </summary>
        public string ImageAllowedExtensions { get; set; }
        /// <summary>
        /// 多媒体允许扩展名
        /// </summary>
        public string MediaAllowedExtensions { get; set; }

        /// <summary>
        /// CMSPrefixKey
        /// </summary>
        public string CMSPrefixKey { get; set; }
    }
}
