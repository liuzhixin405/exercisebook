using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api;


namespace QimenCloud.Api.scenet33hglzv00.Response
{
    /// <summary>
    /// WdgjBackpurchaseListGetResponse.
    /// </summary>
    public class WdgjBackpurchaseListGetResponse : QimenCloudResponse
    {
        /// <summary>
        /// 采购退货单列表
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
	        /// 金额
	        /// </summary>
	        [XmlElement("goodstotal")]
	        public string Goodstotal { get; set; }
	
	        /// <summary>
	        /// 货位
	        /// </summary>
	        [XmlElement("positionname")]
	        public string Positionname { get; set; }
	
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
	        /// 行号
	        /// </summary>
	        [XmlElement("recno")]
	        public string Recno { get; set; }
	
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
	        /// 结算时间
	        /// </summary>
	        [XmlElement("chargedate")]
	        public string Chargedate { get; set; }
	
	        /// <summary>
	        /// 结算金额
	        /// </summary>
	        [XmlElement("chargemoney")]
	        public string Chargemoney { get; set; }
	
	        /// <summary>
	        /// 当前状态('0 待执行 1 已完成 2 被取消')
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
	        /// 物流ID
	        /// </summary>
	        [XmlElement("logisticid")]
	        public string Logisticid { get; set; }
	
	        /// <summary>
	        /// 物流名
	        /// </summary>
	        [XmlElement("logisticname")]
	        public string Logisticname { get; set; }
	
	        /// <summary>
	        /// 物流编号
	        /// </summary>
	        [XmlElement("logisticno")]
	        public string Logisticno { get; set; }
	
	        /// <summary>
	        /// 货款合计
	        /// </summary>
	        [XmlElement("moneytotal")]
	        public string Moneytotal { get; set; }
	
	        /// <summary>
	        /// 单号ID
	        /// </summary>
	        [XmlElement("orderid")]
	        public string Orderid { get; set; }
	
	        /// <summary>
	        /// 退货单编号
	        /// </summary>
	        [XmlElement("orderno")]
	        public string Orderno { get; set; }
	
	        /// <summary>
	        /// 邮资
	        /// </summary>
	        [XmlElement("postagefee")]
	        public string Postagefee { get; set; }
	
	        /// <summary>
	        /// 货运单号
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
	        /// 供货商ID
	        /// </summary>
	        [XmlElement("providerid")]
	        public string Providerid { get; set; }
	
	        /// <summary>
	        /// 供应商名
	        /// </summary>
	        [XmlElement("providername")]
	        public string Providername { get; set; }
	
	        /// <summary>
	        /// 供货商备注
	        /// </summary>
	        [XmlElement("providerremark")]
	        public string Providerremark { get; set; }
	
	        /// <summary>
	        /// 关联采购单号
	        /// </summary>
	        [XmlElement("purchaseorderno")]
	        public string Purchaseorderno { get; set; }
	
	        /// <summary>
	        /// 登记日期
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
	        /// 发货时间
	        /// </summary>
	        [XmlElement("snddate")]
	        public string Snddate { get; set; }
	
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
	        /// 仓库编号
	        /// </summary>
	        [XmlElement("warehouseno")]
	        public string Warehouseno { get; set; }
}

    }
}
