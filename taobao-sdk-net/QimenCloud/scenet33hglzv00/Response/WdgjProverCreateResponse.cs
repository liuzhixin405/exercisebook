using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api;


namespace QimenCloud.Api.scenet33hglzv00.Response
{
    /// <summary>
    /// WdgjProverCreateResponse.
    /// </summary>
    public class WdgjProverCreateResponse : QimenCloudResponse
    {
        /// <summary>
        /// 错误列表
        /// </summary>
        [XmlArray("datalist")]
        [XmlArrayItem("datainfo")]
        public List<DatainfoDomain> Datalist { get; set; }

        /// <summary>
        /// 0 成功 -1 失败 1 部分失败
        /// </summary>
        [XmlElement("returncode")]
        public string Returncode { get; set; }

        /// <summary>
        /// failure
        /// </summary>
        [XmlElement("returnflag")]
        public string Returnflag { get; set; }

        /// <summary>
        /// 成功返回空，失败返回错误信息
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
	        /// 错误信息
	        /// </summary>
	        [XmlElement("errmsg")]
	        public string Errmsg { get; set; }
	
	        /// <summary>
	        /// 供应商名称
	        /// </summary>
	        [XmlElement("providername")]
	        public string Providername { get; set; }
	
	        /// <summary>
	        /// 供应商编号
	        /// </summary>
	        [XmlElement("providerno")]
	        public string Providerno { get; set; }
}

    }
}
