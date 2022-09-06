using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// TopSecretRegisterResponse.
    /// </summary>
    public class TopSecretRegisterResponse : TopResponse
    {
        /// <summary>
        /// 返回操作是否成功
        /// </summary>
        [XmlElement("result")]
        public bool Result { get; set; }

    }
}
