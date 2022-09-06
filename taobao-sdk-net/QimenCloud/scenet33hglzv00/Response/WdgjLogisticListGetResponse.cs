using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api;


namespace QimenCloud.Api.scenet33hglzv00.Response
{
    /// <summary>
    /// WdgjLogisticListGetResponse.
    /// </summary>
    public class WdgjLogisticListGetResponse : QimenCloudResponse
    {
        /// <summary>
        /// 物流公司列表
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
	        /// 是否停用 True/False
	        /// </summary>
	        [XmlElement("bblockup")]
	        public string Bblockup { get; set; }
	
	        /// <summary>
	        /// 打印模版
	        /// </summary>
	        [XmlElement("billstyle")]
	        public string Billstyle { get; set; }
	
	        /// <summary>
	        /// 物流公司联系人
	        /// </summary>
	        [XmlElement("linkman")]
	        public string Linkman { get; set; }
	
	        /// <summary>
	        /// 物流公司ID
	        /// </summary>
	        [XmlElement("logisticid")]
	        public string Logisticid { get; set; }
	
	        /// <summary>
	        /// 物流编号
	        /// </summary>
	        [XmlElement("logisticlistno")]
	        public string Logisticlistno { get; set; }
	
	        /// <summary>
	        /// 物流公司名称
	        /// </summary>
	        [XmlElement("name")]
	        public string Name { get; set; }
	
	        /// <summary>
	        /// 排序
	        /// </summary>
	        [XmlElement("orderpos")]
	        public string Orderpos { get; set; }
	
	        /// <summary>
	        /// 备注
	        /// </summary>
	        [XmlElement("remark")]
	        public string Remark { get; set; }
	
	        /// <summary>
	        /// 物流公司联系电话
	        /// </summary>
	        [XmlElement("tel")]
	        public string Tel { get; set; }
	
	        /// <summary>
	        /// 网址
	        /// </summary>
	        [XmlElement("website")]
	        public string Website { get; set; }
}

    }
}
