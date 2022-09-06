using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api;


namespace QimenCloud.Api.scenet33hglzv00.Response
{
    /// <summary>
    /// WdgjStockoutListGetResponse.
    /// </summary>
    public class WdgjStockoutListGetResponse : QimenCloudResponse
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
	        /// 条码 （条码+附加码）
	        /// </summary>
	        [XmlElement("barcode")]
	        public string Barcode { get; set; }
	
	        /// <summary>
	        /// 数量
	        /// </summary>
	        [XmlElement("goodscount")]
	        public string Goodscount { get; set; }
	
	        /// <summary>
	        /// 货品ID
	        /// </summary>
	        [XmlElement("goodsid")]
	        public string Goodsid { get; set; }
	
	        /// <summary>
	        /// 金额
	        /// </summary>
	        [XmlElement("goodsmoney")]
	        public string Goodsmoney { get; set; }
	
	        /// <summary>
	        /// 品名
	        /// </summary>
	        [XmlElement("goodsname")]
	        public string Goodsname { get; set; }
	
	        /// <summary>
	        /// 货品编号
	        /// </summary>
	        [XmlElement("goodsno")]
	        public string Goodsno { get; set; }
	
	        /// <summary>
	        /// 货位
	        /// </summary>
	        [XmlElement("positionsname")]
	        public string Positionsname { get; set; }
	
	        /// <summary>
	        /// 单价
	        /// </summary>
	        [XmlElement("price")]
	        public string Price { get; set; }
	
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
	        /// 序列号 多个中间用逗号(,)隔开
	        /// </summary>
	        [XmlElement("sn")]
	        public string Sn { get; set; }
	
	        /// <summary>
	        /// 规格ID，单规格为0
	        /// </summary>
	        [XmlElement("specid")]
	        public string Specid { get; set; }
	
	        /// <summary>
	        /// 规格名
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
	        /// 审核人员
	        /// </summary>
	        [XmlElement("chkoperator")]
	        public string Chkoperator { get; set; }
	
	        /// <summary>
	        /// 审核时间
	        /// </summary>
	        [XmlElement("chktime")]
	        public string Chktime { get; set; }
	
	        /// <summary>
	        /// '0 待审核 1 已审核 2 已取消'
	        /// </summary>
	        [XmlElement("curstatus")]
	        public string Curstatus { get; set; }
	
	        /// <summary>
	        /// 货品信息
	        /// </summary>
	        [XmlArray("goodslist")]
	        [XmlArrayItem("goodsinfo")]
	        public List<GoodsinfoDomain> Goodslist { get; set; }
	
	        /// <summary>
	        /// 合计金额
	        /// </summary>
	        [XmlElement("goodstotal")]
	        public string Goodstotal { get; set; }
	
	        /// <summary>
	        /// 操作人
	        /// </summary>
	        [XmlElement("operater")]
	        public string Operater { get; set; }
	
	        /// <summary>
	        /// 关联ID（0 表示无关联）
	        /// </summary>
	        [XmlElement("operationid")]
	        public string Operationid { get; set; }
	
	        /// <summary>
	        /// 0 出库冲销 1 销售出库 4 采购退货 9 调拨出库 10 生产领料
	        /// </summary>
	        [XmlElement("operationtype")]
	        public string Operationtype { get; set; }
	
	        /// <summary>
	        /// 其他费用
	        /// </summary>
	        [XmlElement("otherfee")]
	        public string Otherfee { get; set; }
	
	        /// <summary>
	        /// 出库时间
	        /// </summary>
	        [XmlElement("regtime")]
	        public string Regtime { get; set; }
	
	        /// <summary>
	        /// 备注
	        /// </summary>
	        [XmlElement("remark")]
	        public string Remark { get; set; }
	
	        /// <summary>
	        /// 店铺id
	        /// </summary>
	        [XmlElement("shopid")]
	        public string Shopid { get; set; }
	
	        /// <summary>
	        /// 店铺名
	        /// </summary>
	        [XmlElement("shopname")]
	        public string Shopname { get; set; }
	
	        /// <summary>
	        /// 店铺编号
	        /// </summary>
	        [XmlElement("shopno")]
	        public string Shopno { get; set; }
	
	        /// <summary>
	        /// 出库单ID
	        /// </summary>
	        [XmlElement("stockid")]
	        public string Stockid { get; set; }
	
	        /// <summary>
	        /// 出库单号
	        /// </summary>
	        [XmlElement("stockno")]
	        public string Stockno { get; set; }
	
	        /// <summary>
	        /// 摘要
	        /// </summary>
	        [XmlElement("summary")]
	        public string Summary { get; set; }
	
	        /// <summary>
	        /// 出库类型（出库原因）
	        /// </summary>
	        [XmlElement("thecause")]
	        public string Thecause { get; set; }
	
	        /// <summary>
	        /// 原始单号
	        /// </summary>
	        [XmlElement("tradeno2")]
	        public string Tradeno2 { get; set; }
	
	        /// <summary>
	        /// 出库仓库id
	        /// </summary>
	        [XmlElement("warehouseid")]
	        public string Warehouseid { get; set; }
	
	        /// <summary>
	        /// 出库仓库名
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
