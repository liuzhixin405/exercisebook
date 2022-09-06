using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api;


namespace QimenCloud.Api.scenet33hglzv00.Response
{
    /// <summary>
    /// WdgjTradeListGetResponse.
    /// </summary>
    public class WdgjTradeListGetResponse : QimenCloudResponse
    {
        /// <summary>
        /// 订单列表
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
        /// 响应失败出参判断条件
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
	        /// 是否赠品 True/False
	        /// </summary>
	        [XmlElement("bgift")]
	        public string Bgift { get; set; }
	
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
	        /// 合计应收
	        /// </summary>
	        [XmlElement("alltotal")]
	        public string Alltotal { get; set; }
	
	        /// <summary>
	        /// 0不开发票，1纸质普票，2电子普票，3电子专票，4纸质专票，5冠名发票
	        /// </summary>
	        [XmlElement("binvoice")]
	        public string Binvoice { get; set; }
	
	        /// <summary>
	        /// 取消原因
	        /// </summary>
	        [XmlElement("cancelreason")]
	        public string Cancelreason { get; set; }
	
	        /// <summary>
	        /// 支付单号
	        /// </summary>
	        [XmlElement("chargeid")]
	        public string Chargeid { get; set; }
	
	        /// <summary>
	        /// 结算方式
	        /// </summary>
	        [XmlElement("chargetype")]
	        public string Chargetype { get; set; }
	
	        /// <summary>
	        /// 验货人
	        /// </summary>
	        [XmlElement("chkoperator")]
	        public string Chkoperator { get; set; }
	
	        /// <summary>
	        /// 验货时间
	        /// </summary>
	        [XmlElement("chktime")]
	        public string Chktime { get; set; }
	
	        /// <summary>
	        /// 市
	        /// </summary>
	        [XmlElement("city")]
	        public string City { get; set; }
	
	        /// <summary>
	        /// 交易佣金
	        /// </summary>
	        [XmlElement("commissionvalue")]
	        public string Commissionvalue { get; set; }
	
	        /// <summary>
	        /// 审核人
	        /// </summary>
	        [XmlElement("confirmoperator")]
	        public string Confirmoperator { get; set; }
	
	        /// <summary>
	        /// 审核时间
	        /// </summary>
	        [XmlElement("confirmtime")]
	        public string Confirmtime { get; set; }
	
	        /// <summary>
	        /// 国家
	        /// </summary>
	        [XmlElement("country")]
	        public string Country { get; set; }
	
	        /// <summary>
	        /// 抵扣金额
	        /// </summary>
	        [XmlElement("couponvalue")]
	        public string Couponvalue { get; set; }
	
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
	        /// 顾客id
	        /// </summary>
	        [XmlElement("customerid")]
	        public string Customerid { get; set; }
	
	        /// <summary>
	        /// 买家留言
	        /// </summary>
	        [XmlElement("customerremark")]
	        public string Customerremark { get; set; }
	
	        /// <summary>
	        /// 优惠金额
	        /// </summary>
	        [XmlElement("favourabletotal")]
	        public string Favourabletotal { get; set; }
	
	        /// <summary>
	        /// 冻结原因
	        /// </summary>
	        [XmlElement("freezereason")]
	        public string Freezereason { get; set; }
	
	        /// <summary>
	        /// 货品成本
	        /// </summary>
	        [XmlElement("goodscost")]
	        public string Goodscost { get; set; }
	
	        /// <summary>
	        /// 货品列表
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
	        /// 货品重量
	        /// </summary>
	        [XmlElement("goodsweight")]
	        public string Goodsweight { get; set; }
	
	        /// <summary>
	        /// 发票抬头
	        /// </summary>
	        [XmlElement("invoicetitle")]
	        public string Invoicetitle { get; set; }
	
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
	        /// 物流名
	        /// </summary>
	        [XmlElement("logisticname")]
	        public string Logisticname { get; set; }
	
	        /// <summary>
	        /// 其他成本
	        /// </summary>
	        [XmlElement("othercost")]
	        public string Othercost { get; set; }
	
	        /// <summary>
	        /// 包裹重量
	        /// </summary>
	        [XmlElement("packagedweight")]
	        public string Packagedweight { get; set; }
	
	        /// <summary>
	        /// 打包员
	        /// </summary>
	        [XmlElement("packageoperator")]
	        public string Packageoperator { get; set; }
	
	        /// <summary>
	        /// 拣货员
	        /// </summary>
	        [XmlElement("picker")]
	        public string Picker { get; set; }
	
	        /// <summary>
	        /// 实际邮资
	        /// </summary>
	        [XmlElement("postage")]
	        public string Postage { get; set; }
	
	        /// <summary>
	        /// 应收邮资
	        /// </summary>
	        [XmlElement("postagetotal")]
	        public string Postagetotal { get; set; }
	
	        /// <summary>
	        /// 快递单号
	        /// </summary>
	        [XmlElement("postid")]
	        public string Postid { get; set; }
	
	        /// <summary>
	        /// 供应商ID
	        /// </summary>
	        [XmlElement("providerid")]
	        public string Providerid { get; set; }
	
	        /// <summary>
	        /// 供应商姓名
	        /// </summary>
	        [XmlElement("providername")]
	        public string Providername { get; set; }
	
	        /// <summary>
	        /// 供应商编号
	        /// </summary>
	        [XmlElement("providerno")]
	        public string Providerno { get; set; }
	
	        /// <summary>
	        /// 省
	        /// </summary>
	        [XmlElement("province")]
	        public string Province { get; set; }
	
	        /// <summary>
	        /// 实际结算
	        /// </summary>
	        [XmlElement("recvtotal")]
	        public string Recvtotal { get; set; }
	
	        /// <summary>
	        /// 登记人
	        /// </summary>
	        [XmlElement("regoperator")]
	        public string Regoperator { get; set; }
	
	        /// <summary>
	        /// 登记时间
	        /// </summary>
	        [XmlElement("regtime")]
	        public string Regtime { get; set; }
	
	        /// <summary>
	        /// 客服备注
	        /// </summary>
	        [XmlElement("remark")]
	        public string Remark { get; set; }
	
	        /// <summary>
	        /// 自定义字段1
	        /// </summary>
	        [XmlElement("reserved1")]
	        public string Reserved1 { get; set; }
	
	        /// <summary>
	        /// 自定义字段2
	        /// </summary>
	        [XmlElement("reserved2")]
	        public string Reserved2 { get; set; }
	
	        /// <summary>
	        /// 自定义字段3
	        /// </summary>
	        [XmlElement("reserved3")]
	        public string Reserved3 { get; set; }
	
	        /// <summary>
	        /// 自定义字段4
	        /// </summary>
	        [XmlElement("reserved4")]
	        public string Reserved4 { get; set; }
	
	        /// <summary>
	        /// 业务员
	        /// </summary>
	        [XmlElement("seller")]
	        public string Seller { get; set; }
	
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
	        /// 平台类型
	        /// </summary>
	        [XmlElement("shoptype")]
	        public string Shoptype { get; set; }
	
	        /// <summary>
	        /// 发货人
	        /// </summary>
	        [XmlElement("sndoperator")]
	        public string Sndoperator { get; set; }
	
	        /// <summary>
	        /// 发货时间
	        /// </summary>
	        [XmlElement("sndtime")]
	        public string Sndtime { get; set; }
	
	        /// <summary>
	        /// 收货姓名
	        /// </summary>
	        [XmlElement("sndto")]
	        public string Sndto { get; set; }
	
	        /// <summary>
	        /// 客付税额
	        /// </summary>
	        [XmlElement("taxvalue")]
	        public string Taxvalue { get; set; }
	
	        /// <summary>
	        /// 订单利润
	        /// </summary>
	        [XmlElement("totalprofit")]
	        public string Totalprofit { get; set; }
	
	        /// <summary>
	        /// 区
	        /// </summary>
	        [XmlElement("town")]
	        public string Town { get; set; }
	
	        /// <summary>
	        /// 订单来源
	        /// </summary>
	        [XmlElement("tradefrom")]
	        public string Tradefrom { get; set; }
	
	        /// <summary>
	        /// 单号ID
	        /// </summary>
	        [XmlElement("tradeid")]
	        public string Tradeid { get; set; }
	
	        /// <summary>
	        /// 网名
	        /// </summary>
	        [XmlElement("tradenick")]
	        public string Tradenick { get; set; }
	
	        /// <summary>
	        /// 单号
	        /// </summary>
	        [XmlElement("tradeno")]
	        public string Tradeno { get; set; }
	
	        /// <summary>
	        /// 原始单号
	        /// </summary>
	        [XmlElement("tradeno2")]
	        public string Tradeno2 { get; set; }
	
	        /// <summary>
	        /// 订单状态
	        /// </summary>
	        [XmlElement("tradestatus")]
	        public string Tradestatus { get; set; }
	
	        /// <summary>
	        /// 交易时间
	        /// </summary>
	        [XmlElement("tradetime")]
	        public string Tradetime { get; set; }
	
	        /// <summary>
	        /// 订单类型
	        /// </summary>
	        [XmlElement("tradetype")]
	        public string Tradetype { get; set; }
	
	        /// <summary>
	        /// 仓库id
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
	
	        /// <summary>
	        /// 邮编
	        /// </summary>
	        [XmlElement("zip")]
	        public string Zip { get; set; }
}

    }
}
