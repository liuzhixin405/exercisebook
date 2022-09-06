using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api;


namespace QimenCloud.Api.scenet33hglzv00.Response
{
    /// <summary>
    /// WdgjShopListGetResponse.
    /// </summary>
    public class WdgjShopListGetResponse : QimenCloudResponse
    {
        /// <summary>
        /// 店铺列表
        /// </summary>
        [XmlArray("datalist")]
        [XmlArrayItem("datainfo")]
        public List<DatainfoDomain> Datalist { get; set; }

        /// <summary>
        /// 二级错误码
        /// </summary>
        [XmlElement("returncode")]
        public string Returncode { get; set; }

        /// <summary>
        /// 响应失败出参判断条件
        /// </summary>
        [XmlElement("returnflag")]
        public string Returnflag { get; set; }

        /// <summary>
        /// 二级错误信息
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
	        /// 详细地址
	        /// </summary>
	        [XmlElement("adr")]
	        public string Adr { get; set; }
	
	        /// <summary>
	        /// 店铺是否停用 True/False
	        /// </summary>
	        [XmlElement("bblockup")]
	        public string Bblockup { get; set; }
	
	        /// <summary>
	        /// 发货单模板
	        /// </summary>
	        [XmlElement("billstyle")]
	        public string Billstyle { get; set; }
	
	        /// <summary>
	        /// 结算方式
	        /// </summary>
	        [XmlElement("chargetype")]
	        public string Chargetype { get; set; }
	
	        /// <summary>
	        /// 区市
	        /// </summary>
	        [XmlElement("city")]
	        public string City { get; set; }
	
	        /// <summary>
	        /// 国家
	        /// </summary>
	        [XmlElement("country")]
	        public string Country { get; set; }
	
	        /// <summary>
	        /// 邮箱
	        /// </summary>
	        [XmlElement("email")]
	        public string Email { get; set; }
	
	        /// <summary>
	        /// 联系人
	        /// </summary>
	        [XmlElement("linkman")]
	        public string Linkman { get; set; }
	
	        /// <summary>
	        /// 州省
	        /// </summary>
	        [XmlElement("province")]
	        public string Province { get; set; }
	
	        /// <summary>
	        /// 店铺id
	        /// </summary>
	        [XmlElement("shopid")]
	        public string Shopid { get; set; }
	
	        /// <summary>
	        /// 店铺名称
	        /// </summary>
	        [XmlElement("shopname")]
	        public string Shopname { get; set; }
	
	        /// <summary>
	        /// 店铺编码
	        /// </summary>
	        [XmlElement("shopno")]
	        public string Shopno { get; set; }
	
	        /// <summary>
	        /// 店铺类型
	        /// </summary>
	        [XmlElement("shoptype")]
	        public string Shoptype { get; set; }
	
	        /// <summary>
	        /// 电话
	        /// </summary>
	        [XmlElement("tel")]
	        public string Tel { get; set; }
	
	        /// <summary>
	        /// 区县
	        /// </summary>
	        [XmlElement("town")]
	        public string Town { get; set; }
	
	        /// <summary>
	        /// 仓库id
	        /// </summary>
	        [XmlElement("warehouseid")]
	        public string Warehouseid { get; set; }
	
	        /// <summary>
	        /// 仓库名称
	        /// </summary>
	        [XmlElement("warehousename")]
	        public string Warehousename { get; set; }
	
	        /// <summary>
	        /// 仓库编号
	        /// </summary>
	        [XmlElement("warehouseno")]
	        public string Warehouseno { get; set; }
	
	        /// <summary>
	        /// 网址
	        /// </summary>
	        [XmlElement("website")]
	        public string Website { get; set; }
	
	        /// <summary>
	        /// 邮编
	        /// </summary>
	        [XmlElement("zip")]
	        public string Zip { get; set; }
}

    }
}
