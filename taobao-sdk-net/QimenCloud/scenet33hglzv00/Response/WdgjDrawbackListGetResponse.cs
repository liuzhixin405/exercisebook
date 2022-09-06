using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api;


namespace QimenCloud.Api.scenet33hglzv00.Response
{
    /// <summary>
    /// WdgjDrawbackListGetResponse.
    /// </summary>
    public class WdgjDrawbackListGetResponse : QimenCloudResponse
    {
        /// <summary>
        /// 返回的补偿单列表
        /// </summary>
        [XmlArray("datalist")]
        [XmlArrayItem("datainfo")]
        public List<DatainfoDomain> Datalist { get; set; }

        /// <summary>
        /// 成功0 失败-1
        /// </summary>
        [XmlElement("returncode")]
        public string Returncode { get; set; }

        /// <summary>
        /// 失败的标识
        /// </summary>
        [XmlElement("returnflag")]
        public string Returnflag { get; set; }

        /// <summary>
        /// 成功返回当前返回的条数，失败返回原因
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
	        /// 关联订单的审核时间
	        /// </summary>
	        [XmlElement("chktime")]
	        public string Chktime { get; set; }
	
	        /// <summary>
	        /// 补偿退款单状态 退款状态 0已结算完成 1待收支审核 2应付款待结算 3收支结算完成 4收支已冲销 5取消收支 6应付结算完成 7应付部分结算 8取消应付 9应付已冲销
	        /// </summary>
	        [XmlElement("curstatus")]
	        public string Curstatus { get; set; }
	
	        /// <summary>
	        /// 关联订单的顾客名称 淘系订单不返回
	        /// </summary>
	        [XmlElement("customername")]
	        public string Customername { get; set; }
	
	        /// <summary>
	        /// 关联订单的收货人电话 淘系订单不返回
	        /// </summary>
	        [XmlElement("customerphone")]
	        public string Customerphone { get; set; }
	
	        /// <summary>
	        /// 补偿退款单的经办人
	        /// </summary>
	        [XmlElement("drackbakoperator")]
	        public string Drackbakoperator { get; set; }
	
	        /// <summary>
	        /// 补偿金额
	        /// </summary>
	        [XmlElement("drawbackvalue")]
	        public string Drawbackvalue { get; set; }
	
	        /// <summary>
	        /// 关联订单的货款合计
	        /// </summary>
	        [XmlElement("goodstotal")]
	        public string Goodstotal { get; set; }
	
	        /// <summary>
	        /// 补偿退款单登记时间
	        /// </summary>
	        [XmlElement("logtime")]
	        public string Logtime { get; set; }
	
	        /// <summary>
	        /// 关联的订单的原始单号
	        /// </summary>
	        [XmlElement("origintradeno")]
	        public string Origintradeno { get; set; }
	
	        /// <summary>
	        /// 补偿退款的原因
	        /// </summary>
	        [XmlElement("reason")]
	        public string Reason { get; set; }
	
	        /// <summary>
	        /// 退款方式
	        /// </summary>
	        [XmlElement("refundtype")]
	        public string Refundtype { get; set; }
	
	        /// <summary>
	        /// 补偿退款单的备注
	        /// </summary>
	        [XmlElement("remark")]
	        public string Remark { get; set; }
	
	        /// <summary>
	        /// 关联订单发货时间
	        /// </summary>
	        [XmlElement("sendtime")]
	        public string Sendtime { get; set; }
	
	        /// <summary>
	        /// 店铺名称
	        /// </summary>
	        [XmlElement("shopname")]
	        public string Shopname { get; set; }
	
	        /// <summary>
	        /// 关联店铺的店铺类型
	        /// </summary>
	        [XmlElement("shoptype")]
	        public string Shoptype { get; set; }
	
	        /// <summary>
	        /// 关联的订单号
	        /// </summary>
	        [XmlElement("tradeno")]
	        public string Tradeno { get; set; }
	
	        /// <summary>
	        /// 关联订单交易时间
	        /// </summary>
	        [XmlElement("tradetime")]
	        public string Tradetime { get; set; }
}

    }
}
