using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api;


namespace QimenCloud.Api.scenet33hglzv00.Response
{
    /// <summary>
    /// WdgjOpenApiQimenResponse.
    /// </summary>
    public class WdgjOpenApiQimenResponse : QimenCloudResponse
    {
        /// <summary>
        /// 包裹开放平台的返回数据
        /// </summary>
        [XmlElement("openapires")]
        public string Openapires { get; set; }

        /// <summary>
        /// returncode
        /// </summary>
        [XmlElement("returncode")]
        public string Returncode { get; set; }

        /// <summary>
        /// returnflag
        /// </summary>
        [XmlElement("returnflag")]
        public string Returnflag { get; set; }

        /// <summary>
        /// returninfo
        /// </summary>
        [XmlElement("returninfo")]
        public string Returninfo { get; set; }

    }
}
