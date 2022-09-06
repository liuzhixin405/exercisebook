using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api;


namespace QimenCloud.Api.scenet33hglzv00.Response
{
    /// <summary>
    /// WdgjStockoutConfirmResponse.
    /// </summary>
    public class WdgjStockoutConfirmResponse : QimenCloudResponse
    {
        /// <summary>
        /// 出库单列表
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
/// GoodsinfoDomain Data Structure.
/// </summary>
[Serializable]

public class GoodsinfoDomain : TopObject
{
	        /// <summary>
	        /// 条码+附加码
	        /// </summary>
	        [XmlElement("barcode")]
	        public string Barcode { get; set; }
	
	        /// <summary>
	        /// 数量
	        /// </summary>
	        [XmlElement("goodscount")]
	        public string Goodscount { get; set; }
	
	        /// <summary>
	        /// 货品id
	        /// </summary>
	        [XmlElement("goodsid")]
	        public string Goodsid { get; set; }
	
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
	        /// 规格名
	        /// </summary>
	        [XmlElement("specname")]
	        public string Specname { get; set; }
}

	/// <summary>
/// DatainfoDomain Data Structure.
/// </summary>
[Serializable]

public class DatainfoDomain : TopObject
{
	        /// <summary>
	        /// 确认类型（0：部分出库，1：完全出库）当请求出库数量小于出库数量会产生部分冲销，当请求出库数量大于出库数量会按照实际出库数量出库
	        /// </summary>
	        [XmlElement("confirmtype")]
	        public string Confirmtype { get; set; }
	
	        /// <summary>
	        /// 货品信息(当传入的货品在是在出库的仓库里面，即使不在出库单中也可让此出库单全部出库成功)
	        /// </summary>
	        [XmlArray("goodslist")]
	        [XmlArrayItem("goodsinfo")]
	        public List<GoodsinfoDomain> Goodslist { get; set; }
	
	        /// <summary>
	        /// 货品主键（1：货品编号和规格名称，2：goodsid和specid，3：条码）
	        /// </summary>
	        [XmlElement("keytype")]
	        public string Keytype { get; set; }
	
	        /// <summary>
	        /// 操作员
	        /// </summary>
	        [XmlElement("operater")]
	        public string Operater { get; set; }
	
	        /// <summary>
	        /// 管家出库单号
	        /// </summary>
	        [XmlElement("orderno")]
	        public string Orderno { get; set; }
}

    }
}
