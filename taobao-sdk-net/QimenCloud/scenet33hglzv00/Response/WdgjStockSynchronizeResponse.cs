using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api;


namespace QimenCloud.Api.scenet33hglzv00.Response
{
    /// <summary>
    /// WdgjStockSynchronizeResponse.
    /// </summary>
    public class WdgjStockSynchronizeResponse : QimenCloudResponse
    {
        /// <summary>
        /// 部分失败错误列表
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
	        /// 条码
	        /// </summary>
	        [XmlElement("barcode")]
	        public string Barcode { get; set; }
	
	        /// <summary>
	        /// 同步错误消息
	        /// </summary>
	        [XmlElement("errmsg")]
	        public string Errmsg { get; set; }
	
	        /// <summary>
	        /// 货品id
	        /// </summary>
	        [XmlElement("goodsid")]
	        public string Goodsid { get; set; }
	
	        /// <summary>
	        /// 货品名称
	        /// </summary>
	        [XmlElement("goodsname")]
	        public string Goodsname { get; set; }
	
	        /// <summary>
	        /// 货品编号
	        /// </summary>
	        [XmlElement("goodsno")]
	        public string Goodsno { get; set; }
	
	        /// <summary>
	        /// 规格id
	        /// </summary>
	        [XmlElement("specid")]
	        public string Specid { get; set; }
	
	        /// <summary>
	        /// 规格
	        /// </summary>
	        [XmlElement("specname")]
	        public string Specname { get; set; }
}

    }
}
