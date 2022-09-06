using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api;


namespace QimenCloud.Api.scenet33hglzv00.Response
{
    /// <summary>
    /// WdgjStockinListGetResponse.
    /// </summary>
    public class WdgjStockinListGetResponse : QimenCloudResponse
    {
        /// <summary>
        /// 入库单信息
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
	        /// 序列号
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
	        /// 汇率
	        /// </summary>
	        [XmlElement("currencyrate")]
	        public string Currencyrate { get; set; }
	
	        /// <summary>
	        /// 币种
	        /// </summary>
	        [XmlElement("currencytype")]
	        public string Currencytype { get; set; }
	
	        /// <summary>
	        /// 0 待审核 1 已审核 2 已取消'
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
	        /// 货运费用
	        /// </summary>
	        [XmlElement("logisticfee")]
	        public string Logisticfee { get; set; }
	
	        /// <summary>
	        /// 物流id
	        /// </summary>
	        [XmlElement("logisticid")]
	        public string Logisticid { get; set; }
	
	        /// <summary>
	        /// 物流编号
	        /// </summary>
	        [XmlElement("logisticlistno")]
	        public string Logisticlistno { get; set; }
	
	        /// <summary>
	        /// 物流名称
	        /// </summary>
	        [XmlElement("logisticname")]
	        public string Logisticname { get; set; }
	
	        /// <summary>
	        /// 登记人
	        /// </summary>
	        [XmlElement("operater")]
	        public string Operater { get; set; }
	
	        /// <summary>
	        /// 关联ID（0 表示无关联）
	        /// </summary>
	        [XmlElement("operationid")]
	        public string Operationid { get; set; }
	
	        /// <summary>
	        /// 单据类型（2 （采购入库，采购入库冲销） 3退货入库 5库存盘点 9调拨入库 10 生产入库）
	        /// </summary>
	        [XmlElement("operationtype")]
	        public string Operationtype { get; set; }
	
	        /// <summary>
	        /// 其他费用
	        /// </summary>
	        [XmlElement("otherfee")]
	        public string Otherfee { get; set; }
	
	        /// <summary>
	        /// 物流单号
	        /// </summary>
	        [XmlElement("postid")]
	        public string Postid { get; set; }
	
	        /// <summary>
	        /// 折扣
	        /// </summary>
	        [XmlElement("pricedis")]
	        public string Pricedis { get; set; }
	
	        /// <summary>
	        /// 执行价格
	        /// </summary>
	        [XmlElement("pricespec")]
	        public string Pricespec { get; set; }
	
	        /// <summary>
	        /// 供应商id
	        /// </summary>
	        [XmlElement("providerid")]
	        public string Providerid { get; set; }
	
	        /// <summary>
	        /// 供应商名
	        /// </summary>
	        [XmlElement("providername")]
	        public string Providername { get; set; }
	
	        /// <summary>
	        /// 供应商编号
	        /// </summary>
	        [XmlElement("providerno")]
	        public string Providerno { get; set; }
	
	        /// <summary>
	        /// 登记时间
	        /// </summary>
	        [XmlElement("regtime")]
	        public string Regtime { get; set; }
	
	        /// <summary>
	        /// 备注
	        /// </summary>
	        [XmlElement("remark")]
	        public string Remark { get; set; }
	
	        /// <summary>
	        /// 入库单ID
	        /// </summary>
	        [XmlElement("stockid")]
	        public string Stockid { get; set; }
	
	        /// <summary>
	        /// 入库单号
	        /// </summary>
	        [XmlElement("stockno")]
	        public string Stockno { get; set; }
	
	        /// <summary>
	        /// 入库类型（入库原因）
	        /// </summary>
	        [XmlElement("thecause")]
	        public string Thecause { get; set; }
	
	        /// <summary>
	        /// 入库仓库id
	        /// </summary>
	        [XmlElement("warehouseid")]
	        public string Warehouseid { get; set; }
	
	        /// <summary>
	        /// 入库仓库名
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
