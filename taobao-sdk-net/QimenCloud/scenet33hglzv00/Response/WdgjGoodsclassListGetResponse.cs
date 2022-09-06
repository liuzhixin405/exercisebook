using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api;


namespace QimenCloud.Api.scenet33hglzv00.Response
{
    /// <summary>
    /// WdgjGoodsclassListGetResponse.
    /// </summary>
    public class WdgjGoodsclassListGetResponse : QimenCloudResponse
    {
        /// <summary>
        /// 货品分类列表
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
	        /// 类别ID
	        /// </summary>
	        [XmlElement("classid")]
	        public string Classid { get; set; }
	
	        /// <summary>
	        /// 规格自编码
	        /// </summary>
	        [XmlElement("classid2")]
	        public string Classid2 { get; set; }
	
	        /// <summary>
	        /// 第几级
	        /// </summary>
	        [XmlElement("classlevel")]
	        public string Classlevel { get; set; }
	
	        /// <summary>
	        /// 类别名
	        /// </summary>
	        [XmlElement("classname")]
	        public string Classname { get; set; }
	
	        /// <summary>
	        /// 父类别ID
	        /// </summary>
	        [XmlElement("fatherid")]
	        public string Fatherid { get; set; }
	
	        /// <summary>
	        /// 排序
	        /// </summary>
	        [XmlElement("orderpos")]
	        public string Orderpos { get; set; }
}

    }
}
