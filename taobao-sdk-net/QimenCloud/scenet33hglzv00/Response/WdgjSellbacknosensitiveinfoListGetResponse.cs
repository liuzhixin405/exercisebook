using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api;


namespace QimenCloud.Api.scenet33hglzv00.Response
{
    /// <summary>
    /// WdgjSellbacknosensitiveinfoListGetResponse.
    /// </summary>
    public class WdgjSellbacknosensitiveinfoListGetResponse : QimenCloudResponse
    {
        /// <summary>
        /// 退换单列表
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
	        /// 是否组合装 True/False
	        /// </summary>
	        [XmlElement("bfit")]
	        public string Bfit { get; set; }
	
	        /// <summary>
	        /// 是否赠品 True/False（换货没有这个字段）
	        /// </summary>
	        [XmlElement("bgift")]
	        public string Bgift { get; set; }
	
	        /// <summary>
	        /// 数量
	        /// </summary>
	        [XmlElement("goodscount")]
	        public string Goodscount { get; set; }
	
	        /// <summary>
	        /// goodsid
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
	        /// 地址
	        /// </summary>
	        [XmlElement("adr")]
	        public string Adr { get; set; }
	
	        /// <summary>
	        /// 结算时间
	        /// </summary>
	        [XmlElement("chargetime")]
	        public string Chargetime { get; set; }
	
	        /// <summary>
	        /// 市
	        /// </summary>
	        [XmlElement("city")]
	        public string City { get; set; }
	
	        /// <summary>
	        /// 国家
	        /// </summary>
	        [XmlElement("country")]
	        public string Country { get; set; }
	
	        /// <summary>
	        /// 会员id
	        /// </summary>
	        [XmlElement("customerid")]
	        public string Customerid { get; set; }
	
	        /// <summary>
	        /// 退货商品明细
	        /// </summary>
	        [XmlArray("goodslist")]
	        [XmlArrayItem("goodsinfo")]
	        public List<GoodsinfoDomain> Goodslist { get; set; }
	
	        /// <summary>
	        /// 换货商品明细
	        /// </summary>
	        [XmlArray("goodslist2")]
	        [XmlArrayItem("goodsinfo")]
	        public List<GoodsinfoDomain> Goodslist2 { get; set; }
	
	        /// <summary>
	        /// 物流id
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
	        /// 换货管家关联订单号
	        /// </summary>
	        [XmlElement("newno")]
	        public string Newno { get; set; }
	
	        /// <summary>
	        /// 网名
	        /// </summary>
	        [XmlElement("nickname")]
	        public string Nickname { get; set; }
	
	        /// <summary>
	        /// 管家原关联订单号
	        /// </summary>
	        [XmlElement("oldno")]
	        public string Oldno { get; set; }
	
	        /// <summary>
	        /// 原订单的地址
	        /// </summary>
	        [XmlElement("orderadr")]
	        public string Orderadr { get; set; }
	
	        /// <summary>
	        /// 应收邮资
	        /// </summary>
	        [XmlElement("postagetotal")]
	        public string Postagetotal { get; set; }
	
	        /// <summary>
	        /// 省
	        /// </summary>
	        [XmlElement("province")]
	        public string Province { get; set; }
	
	        /// <summary>
	        /// 原始单号
	        /// </summary>
	        [XmlElement("rawno")]
	        public string Rawno { get; set; }
	
	        /// <summary>
	        /// 收货时间
	        /// </summary>
	        [XmlElement("rcvtime")]
	        public string Rcvtime { get; set; }
	
	        /// <summary>
	        /// 取消原因
	        /// </summary>
	        [XmlElement("reason")]
	        public string Reason { get; set; }
	
	        /// <summary>
	        /// 结算金额
	        /// </summary>
	        [XmlElement("recvtotal")]
	        public string Recvtotal { get; set; }
	
	        /// <summary>
	        /// 登记时间
	        /// </summary>
	        [XmlElement("regtime")]
	        public string Regtime { get; set; }
	
	        /// <summary>
	        /// 卖家备注
	        /// </summary>
	        [XmlElement("remark")]
	        public string Remark { get; set; }
	
	        /// <summary>
	        /// 应退合计
	        /// </summary>
	        [XmlElement("returntotal")]
	        public string Returntotal { get; set; }
	
	        /// <summary>
	        /// 类型“退”只有退货；“换”有换货
	        /// </summary>
	        [XmlElement("returntype")]
	        public string Returntype { get; set; }
	
	        /// <summary>
	        /// 业务员
	        /// </summary>
	        [XmlElement("seller")]
	        public string Seller { get; set; }
	
	        /// <summary>
	        /// 原店铺名
	        /// </summary>
	        [XmlElement("shopname")]
	        public string Shopname { get; set; }
	
	        /// <summary>
	        /// 换货店铺名
	        /// </summary>
	        [XmlElement("shopname2")]
	        public string Shopname2 { get; set; }
	
	        /// <summary>
	        /// 原店铺编号
	        /// </summary>
	        [XmlElement("shopno")]
	        public string Shopno { get; set; }
	
	        /// <summary>
	        /// 换货店铺编号
	        /// </summary>
	        [XmlElement("shopno2")]
	        public string Shopno2 { get; set; }
	
	        /// <summary>
	        /// 收货人
	        /// </summary>
	        [XmlElement("sndto")]
	        public string Sndto { get; set; }
	
	        /// <summary>
	        /// 手机号
	        /// </summary>
	        [XmlElement("tel")]
	        public string Tel { get; set; }
	
	        /// <summary>
	        /// 退货金额
	        /// </summary>
	        [XmlElement("totalpay")]
	        public string Totalpay { get; set; }
	
	        /// <summary>
	        /// 换货金额
	        /// </summary>
	        [XmlElement("totalrcv")]
	        public string Totalrcv { get; set; }
	
	        /// <summary>
	        /// 区
	        /// </summary>
	        [XmlElement("town")]
	        public string Town { get; set; }
	
	        /// <summary>
	        /// 退换单ID
	        /// </summary>
	        [XmlElement("tradeid")]
	        public string Tradeid { get; set; }
	
	        /// <summary>
	        /// 退换单号
	        /// </summary>
	        [XmlElement("tradeno")]
	        public string Tradeno { get; set; }
	
	        /// <summary>
	        /// 退换单状态 0 待收货 1 待结算 2 被取消 3 已完成 4 待审核
	        /// </summary>
	        [XmlElement("tradestatus")]
	        public string Tradestatus { get; set; }
	
	        /// <summary>
	        /// 原订单类型
	        /// </summary>
	        [XmlElement("tradetype")]
	        public string Tradetype { get; set; }
	
	        /// <summary>
	        /// 退货仓库id
	        /// </summary>
	        [XmlElement("warehouseid")]
	        public string Warehouseid { get; set; }
	
	        /// <summary>
	        /// 换货仓库id
	        /// </summary>
	        [XmlElement("warehouseid2")]
	        public string Warehouseid2 { get; set; }
	
	        /// <summary>
	        /// 退货仓库名
	        /// </summary>
	        [XmlElement("warehousename")]
	        public string Warehousename { get; set; }
	
	        /// <summary>
	        /// 换货仓库名
	        /// </summary>
	        [XmlElement("warehousename2")]
	        public string Warehousename2 { get; set; }
	
	        /// <summary>
	        /// 退货仓库编号
	        /// </summary>
	        [XmlElement("warehouseno")]
	        public string Warehouseno { get; set; }
	
	        /// <summary>
	        /// 换货仓库编号
	        /// </summary>
	        [XmlElement("warehouseno2")]
	        public string Warehouseno2 { get; set; }
	
	        /// <summary>
	        /// 邮编
	        /// </summary>
	        [XmlElement("zip")]
	        public string Zip { get; set; }
}

    }
}
