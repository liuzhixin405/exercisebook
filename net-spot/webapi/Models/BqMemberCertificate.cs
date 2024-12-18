using System;
using System.Collections.Generic;

namespace webapi
{
    public partial class BqMemberCertificate
    {
        public int Id { get; set; }
        /// <summary>
        /// 大赛活动名称
        /// </summary>
        public string? ActivityName { get; set; }
        /// <summary>
        /// 证书类型id
        /// </summary>
        public int CertificateId { get; set; }
        /// <summary>
        /// 证书荣誉名称
        /// </summary>
        public string? CertificateName { get; set; }
        /// <summary>
        /// 颁发时间
        /// </summary>
        public int IssueTime { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public int UserId { get; set; }
    }
}
