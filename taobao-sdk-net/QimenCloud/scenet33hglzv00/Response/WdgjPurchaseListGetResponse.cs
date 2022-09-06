using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api;


namespace QimenCloud.Api.scenet33hglzv00.Response
{
    /// <summary>
    /// WdgjPurchaseListGetResponse.
    /// </summary>
    public class WdgjPurchaseListGetResponse : QimenCloudResponse
    {
        /// <summary>
        /// 采购单列表
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
	        /// 到货量
	        /// </summary>
	        [XmlElement("arrivecount")]
	        public string Arrivecount { get; set; }
	
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
	        /// 单价
	        /// </summary>
	        [XmlElement("price")]
	        public string Price { get; set; }
	
	        /// <summary>
	        /// 计划到货量
	        /// </summary>
	        [XmlElement("rcvcount")]
	        public string Rcvcount { get; set; }
	
	        /// <summary>
	        /// 到货日期
	        /// </summary>
	        [XmlElement("rcvdate")]
	        public string Rcvdate { get; set; }
	
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
	        /// 可用库存
	        /// </summary>
	        [XmlElement("stock")]
	        public string Stock { get; set; }
	
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
	        /// 审核时间
	        /// </summary>
	        [XmlElement("chkdate")]
	        public string Chkdate { get; set; }
	
	        /// <summary>
	        /// 审核人
	        /// </summary>
	        [XmlElement("chkoperator")]
	        public string Chkoperator { get; set; }
	
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
	        /// 当前状态(0 待审核 1 执行中 2 被取消 3 已完成 4 被终止)
	        /// </summary>
	        [XmlElement("curstatus")]
	        public string Curstatus { get; set; }
	
	        /// <summary>
	        /// 采购单明细
	        /// </summary>
	        [XmlArray("goodslist")]
	        [XmlArrayItem("goodsinfo")]
	        public List<GoodsinfoDomain> Goodslist { get; set; }
	
	        /// <summary>
	        /// 货款合计
	        /// </summary>
	        [XmlElement("goodstotal")]
	        public string Goodstotal { get; set; }
	
	        /// <summary>
	        /// 发票号码
	        /// </summary>
	        [XmlElement("invoiceno")]
	        public string Invoiceno { get; set; }
	
	        /// <summary>
	        /// 开票方式 0：不开票 1：开具发票
	        /// </summary>
	        [XmlElement("isinvoice")]
	        public string Isinvoice { get; set; }
	
	        /// <summary>
	        /// 采购单ID
	        /// </summary>
	        [XmlElement("orderid")]
	        public string Orderid { get; set; }
	
	        /// <summary>
	        /// 采购单编号
	        /// </summary>
	        [XmlElement("orderno")]
	        public string Orderno { get; set; }
	
	        /// <summary>
	        /// 其他费用
	        /// </summary>
	        [XmlElement("otherfee")]
	        public string Otherfee { get; set; }
	
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
	        /// 采购单批次
	        /// </summary>
	        [XmlElement("printexpress")]
	        public string Printexpress { get; set; }
	
	        /// <summary>
	        /// 供货商ID
	        /// </summary>
	        [XmlElement("providerid")]
	        public string Providerid { get; set; }
	
	        /// <summary>
	        /// 供货商名称
	        /// </summary>
	        [XmlElement("providername")]
	        public string Providername { get; set; }
	
	        /// <summary>
	        /// 供货商编码
	        /// </summary>
	        [XmlElement("providerno")]
	        public string Providerno { get; set; }
	
	        /// <summary>
	        /// 供货商备注
	        /// </summary>
	        [XmlElement("providerremark")]
	        public string Providerremark { get; set; }
	
	        /// <summary>
	        /// 预计到货时间
	        /// </summary>
	        [XmlElement("rcvdate")]
	        public string Rcvdate { get; set; }
	
	        /// <summary>
	        /// 开单日期
	        /// </summary>
	        [XmlElement("regdate")]
	        public string Regdate { get; set; }
	
	        /// <summary>
	        /// 经办人
	        /// </summary>
	        [XmlElement("regoperator")]
	        public string Regoperator { get; set; }
	
	        /// <summary>
	        /// 备注
	        /// </summary>
	        [XmlElement("remark")]
	        public string Remark { get; set; }
	
	        /// <summary>
	        /// 仓库ID
	        /// </summary>
	        [XmlElement("warehouseid")]
	        public string Warehouseid { get; set; }
	
	        /// <summary>
	        /// 仓库名
	        /// </summary>
	        [XmlElement("warehousename")]
	        public string Warehousename { get; set; }
	
	        /// <summary>
	        /// 仓库编码
	        /// </summary>
	        [XmlElement("warehouseno")]
	        public string Warehouseno { get; set; }
}

    }
}
