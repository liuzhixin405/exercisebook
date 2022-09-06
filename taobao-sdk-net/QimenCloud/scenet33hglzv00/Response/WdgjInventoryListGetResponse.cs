using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api;


namespace QimenCloud.Api.scenet33hglzv00.Response
{
    /// <summary>
    /// WdgjInventoryListGetResponse.
    /// </summary>
    public class WdgjInventoryListGetResponse : QimenCloudResponse
    {
        /// <summary>
        /// 盘点单列表
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
/// GoodsinfoDomain Data Structure.
/// </summary>
[Serializable]

public class GoodsinfoDomain : TopObject
{
	        /// <summary>
	        /// 条形码+附加码
	        /// </summary>
	        [XmlElement("barcode")]
	        public string Barcode { get; set; }
	
	        /// <summary>
	        /// 盘点量
	        /// </summary>
	        [XmlElement("countchk")]
	        public string Countchk { get; set; }
	
	        /// <summary>
	        /// 库存量
	        /// </summary>
	        [XmlElement("countpre")]
	        public string Countpre { get; set; }
	
	        /// <summary>
	        /// 货品ID
	        /// </summary>
	        [XmlElement("goodsid")]
	        public string Goodsid { get; set; }
	
	        /// <summary>
	        /// 商品名
	        /// </summary>
	        [XmlElement("goodsname")]
	        public string Goodsname { get; set; }
	
	        /// <summary>
	        /// 货品编码
	        /// </summary>
	        [XmlElement("goodsno")]
	        public string Goodsno { get; set; }
	
	        /// <summary>
	        /// 盈亏量
	        /// </summary>
	        [XmlElement("losscount")]
	        public string Losscount { get; set; }
	
	        /// <summary>
	        /// 盈亏额
	        /// </summary>
	        [XmlElement("lossmoney")]
	        public string Lossmoney { get; set; }
	
	        /// <summary>
	        /// 记录id（唯一）
	        /// </summary>
	        [XmlElement("recid")]
	        public string Recid { get; set; }
	
	        /// <summary>
	        /// 备注
	        /// </summary>
	        [XmlElement("remark")]
	        public string Remark { get; set; }
	
	        /// <summary>
	        /// 规格ID
	        /// </summary>
	        [XmlElement("specid")]
	        public string Specid { get; set; }
	
	        /// <summary>
	        /// 规格名称
	        /// </summary>
	        [XmlElement("specname")]
	        public string Specname { get; set; }
	
	        /// <summary>
	        /// 单位
	        /// </summary>
	        [XmlElement("unit")]
	        public string Unit { get; set; }
}

	/// <summary>
/// DatainfoDomain Data Structure.
/// </summary>
[Serializable]

public class DatainfoDomain : TopObject
{
	        /// <summary>
	        /// 盘点单ID
	        /// </summary>
	        [XmlElement("billid")]
	        public string Billid { get; set; }
	
	        /// <summary>
	        /// 创建时间
	        /// </summary>
	        [XmlElement("createdate")]
	        public string Createdate { get; set; }
	
	        /// <summary>
	        /// 当前状态('0 待盘点 1 已盘点 2 被取消')
	        /// </summary>
	        [XmlElement("curstatus")]
	        public string Curstatus { get; set; }
	
	        /// <summary>
	        /// 盘点单明细
	        /// </summary>
	        [XmlArray("goodslist")]
	        [XmlArrayItem("goodsinfo")]
	        public List<GoodsinfoDomain> Goodslist { get; set; }
	
	        /// <summary>
	        /// 盈亏数量
	        /// </summary>
	        [XmlElement("losscount")]
	        public string Losscount { get; set; }
	
	        /// <summary>
	        /// 盈亏金额
	        /// </summary>
	        [XmlElement("lossmoney")]
	        public string Lossmoney { get; set; }
	
	        /// <summary>
	        /// 创建人
	        /// </summary>
	        [XmlElement("operater")]
	        public string Operater { get; set; }
	
	        /// <summary>
	        /// 盘点人
	        /// </summary>
	        [XmlElement("operator2")]
	        public string Operator2 { get; set; }
	
	        /// <summary>
	        /// 盘点时间
	        /// </summary>
	        [XmlElement("overdate")]
	        public string Overdate { get; set; }
	
	        /// <summary>
	        /// 摘要
	        /// </summary>
	        [XmlElement("summary")]
	        public string Summary { get; set; }
	
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
