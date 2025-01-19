using System;
using System.Collections.Generic;

namespace webapi
{
    public partial class BqCertificateType
    {
        public int Id { get; set; }
        /// <summary>
        /// 证书荣誉名称
        /// </summary>
        public string? CertificateName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public int CreateTime { get; set; }
    }
}
