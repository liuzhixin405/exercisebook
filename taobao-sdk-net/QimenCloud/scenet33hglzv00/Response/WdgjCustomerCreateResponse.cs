using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api;


namespace QimenCloud.Api.scenet33hglzv00.Response
{
    /// <summary>
    /// WdgjCustomerCreateResponse.
    /// </summary>
    public class WdgjCustomerCreateResponse : QimenCloudResponse
    {
        /// <summary>
        /// 错误信息
        /// </summary>
        [XmlArray("datalist")]
        [XmlArrayItem("goodsinfo")]
        public List<GoodsinfoDomain> Datalist { get; set; }

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
/// GoodsinfoDomain Data Structure.
/// </summary>
[Serializable]

public class GoodsinfoDomain : TopObject
{
	        /// <summary>
	        /// 错误消息
	        /// </summary>
	        [XmlElement("errmsg")]
	        public string Errmsg { get; set; }
	
	        /// <summary>
	        /// 网名（昵称）
	        /// </summary>
	        [XmlElement("receivernick")]
	        public string Receivernick { get; set; }
}

    }
}
