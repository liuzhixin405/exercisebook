using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api;


namespace QimenCloud.Api.scenet33hglzv00.Response
{
    /// <summary>
    /// WdgjRequisitionCreateResponse.
    /// </summary>
    public class WdgjRequisitionCreateResponse : QimenCloudResponse
    {
        /// <summary>
        /// 部分失败原因
        /// </summary>
        [XmlArray("datalist")]
        [XmlArrayItem("datainfo")]
        public List<DatainfoDomain> Datalist { get; set; }

        /// <summary>
        /// 0 成功 -1 失败
        /// </summary>
        [XmlElement("returncode")]
        public string Returncode { get; set; }

        /// <summary>
        /// failure
        /// </summary>
        [XmlElement("returnflag")]
        public string Returnflag { get; set; }

        /// <summary>
        /// 成功返条数，失败返回错误信息
        /// </summary>
        [XmlElement("returninfo")]
        public string Returninfo { get; set; }

	/// <summary>
/// DatainfoDomain Data Structure.
/// </summary>
[Serializable]

public class DatainfoDomain : TopObject
{
	        /// <summary>
	        /// 错误原因
	        /// </summary>
	        [XmlElement("errmsg")]
	        public string Errmsg { get; set; }
	
	        /// <summary>
	        /// 单号
	        /// </summary>
	        [XmlElement("orderno")]
	        public string Orderno { get; set; }
}

    }
}
