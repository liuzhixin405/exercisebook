using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api;


namespace QimenCloud.Api.scenet33hglzv00.Response
{
    /// <summary>
    /// WdgjStockListGetResponse.
    /// </summary>
    public class WdgjStockListGetResponse : QimenCloudResponse
    {
        /// <summary>
        /// 库存列表(具体到货位)
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
/// PositioninfoDomain Data Structure.
/// </summary>
[Serializable]

public class PositioninfoDomain : TopObject
{
	        /// <summary>
	        /// 货位状态
	        /// </summary>
	        [XmlElement("curstatus")]
	        public string Curstatus { get; set; }
	
	        /// <summary>
	        /// 货位备注
	        /// </summary>
	        [XmlElement("positionremark")]
	        public string Positionremark { get; set; }
	
	        /// <summary>
	        /// 货位名
	        /// </summary>
	        [XmlElement("positionsname")]
	        public string Positionsname { get; set; }
	
	        /// <summary>
	        /// 货位库存
	        /// </summary>
	        [XmlElement("stock")]
	        public string Stock { get; set; }
	
	        /// <summary>
	        /// 仓库名称
	        /// </summary>
	        [XmlElement("warehousename")]
	        public string Warehousename { get; set; }
}

	/// <summary>
/// DatainfoDomain Data Structure.
/// </summary>
[Serializable]

public class DatainfoDomain : TopObject
{
	        /// <summary>
	        /// 条码+附加码
	        /// </summary>
	        [XmlElement("barcode")]
	        public string Barcode { get; set; }
	
	        /// <summary>
	        /// 调入在途
	        /// </summary>
	        [XmlElement("dbincount")]
	        public string Dbincount { get; set; }
	
	        /// <summary>
	        /// 调出在途
	        /// </summary>
	        [XmlElement("dboutcount")]
	        public string Dboutcount { get; set; }
	
	        /// <summary>
	        /// 商品ID
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
	        /// 订购量
	        /// </summary>
	        [XmlElement("ordercount")]
	        public string Ordercount { get; set; }
	
	        /// <summary>
	        /// 货位库存列表
	        /// </summary>
	        [XmlArray("positionlist")]
	        [XmlArrayItem("positioninfo")]
	        public List<PositioninfoDomain> Positionlist { get; set; }
	
	        /// <summary>
	        /// 成本价
	        /// </summary>
	        [XmlElement("pricecost")]
	        public string Pricecost { get; set; }
	
	        /// <summary>
	        /// 采购在途
	        /// </summary>
	        [XmlElement("purchasecount")]
	        public string Purchasecount { get; set; }
	
	        /// <summary>
	        /// 采购计划数
	        /// </summary>
	        [XmlElement("purchaseplan")]
	        public string Purchaseplan { get; set; }
	
	        /// <summary>
	        /// 待发货数
	        /// </summary>
	        [XmlElement("sndcount")]
	        public string Sndcount { get; set; }
	
	        /// <summary>
	        /// 规格ID
	        /// </summary>
	        [XmlElement("specid")]
	        public string Specid { get; set; }
	
	        /// <summary>
	        /// 规格
	        /// </summary>
	        [XmlElement("specname")]
	        public string Specname { get; set; }
	
	        /// <summary>
	        /// 实际库存
	        /// </summary>
	        [XmlElement("stock")]
	        public string Stock { get; set; }
	
	        /// <summary>
	        /// 可订购
	        /// </summary>
	        [XmlElement("stock3")]
	        public string Stock3 { get; set; }
	
	        /// <summary>
	        /// 仓库ID
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
}

    }
}
