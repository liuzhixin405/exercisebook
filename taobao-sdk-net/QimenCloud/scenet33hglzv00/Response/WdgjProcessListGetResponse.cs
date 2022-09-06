using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api;


namespace QimenCloud.Api.scenet33hglzv00.Response
{
    /// <summary>
    /// WdgjProcessListGetResponse.
    /// </summary>
    public class WdgjProcessListGetResponse : QimenCloudResponse
    {
        /// <summary>
        /// 生产单列表
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
	        /// 退料
	        /// </summary>
	        [XmlElement("backcount")]
	        public string Backcount { get; set; }
	
	        /// <summary>
	        /// 条码 （条码+附加码）
	        /// </summary>
	        [XmlElement("barcode")]
	        public string Barcode { get; set; }
	
	        /// <summary>
	        /// 领料
	        /// </summary>
	        [XmlElement("getcount")]
	        public string Getcount { get; set; }
	
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
	        /// BOM项目
	        /// </summary>
	        [XmlElement("bomname")]
	        public string Bomname { get; set; }
	
	        /// <summary>
	        /// 完工率
	        /// </summary>
	        [XmlElement("completionrate")]
	        public string Completionrate { get; set; }
	
	        /// <summary>
	        /// 状态
	        /// </summary>
	        [XmlElement("curstatus")]
	        public string Curstatus { get; set; }
	
	        /// <summary>
	        /// 处理时间
	        /// </summary>
	        [XmlElement("days")]
	        public string Days { get; set; }
	
	        /// <summary>
	        /// 已派工数
	        /// </summary>
	        [XmlElement("dispatchnum")]
	        public string Dispatchnum { get; set; }
	
	        /// <summary>
	        /// 生产备准
	        /// </summary>
	        [XmlElement("exeremark")]
	        public string Exeremark { get; set; }
	
	        /// <summary>
	        /// 数量
	        /// </summary>
	        [XmlElement("goodscount")]
	        public string Goodscount { get; set; }
	
	        /// <summary>
	        /// 货品信息
	        /// </summary>
	        [XmlArray("goodslist")]
	        [XmlArrayItem("goodsinfo")]
	        public List<GoodsinfoDomain> Goodslist { get; set; }
	
	        /// <summary>
	        /// 货品名
	        /// </summary>
	        [XmlElement("goodsname")]
	        public string Goodsname { get; set; }
	
	        /// <summary>
	        /// 货品编号
	        /// </summary>
	        [XmlElement("goodsno")]
	        public string Goodsno { get; set; }
	
	        /// <summary>
	        /// 要求
	        /// </summary>
	        [XmlElement("needs")]
	        public string Needs { get; set; }
	
	        /// <summary>
	        /// 关联采购单号
	        /// </summary>
	        [XmlElement("orderno")]
	        public string Orderno { get; set; }
	
	        /// <summary>
	        /// 完成数量
	        /// </summary>
	        [XmlElement("overcount")]
	        public string Overcount { get; set; }
	
	        /// <summary>
	        /// 完工时间
	        /// </summary>
	        [XmlElement("overtime")]
	        public string Overtime { get; set; }
	
	        /// <summary>
	        /// 建单时间
	        /// </summary>
	        [XmlElement("posttime")]
	        public string Posttime { get; set; }
	
	        /// <summary>
	        /// 加工方式
	        /// </summary>
	        [XmlElement("processtype")]
	        public string Processtype { get; set; }
	
	        /// <summary>
	        /// 生产订单号
	        /// </summary>
	        [XmlElement("produceid")]
	        public string Produceid { get; set; }
	
	        /// <summary>
	        /// 供应商名
	        /// </summary>
	        [XmlElement("providername")]
	        public string Providername { get; set; }
	
	        /// <summary>
	        /// 登记人
	        /// </summary>
	        [XmlElement("regoperator")]
	        public string Regoperator { get; set; }
	
	        /// <summary>
	        /// 备注
	        /// </summary>
	        [XmlElement("remark")]
	        public string Remark { get; set; }
	
	        /// <summary>
	        /// 规格
	        /// </summary>
	        [XmlElement("specname")]
	        public string Specname { get; set; }
	
	        /// <summary>
	        /// 生产负责人
	        /// </summary>
	        [XmlElement("staffname")]
	        public string Staffname { get; set; }
	
	        /// <summary>
	        /// 销售订单号
	        /// </summary>
	        [XmlElement("tradeno")]
	        public string Tradeno { get; set; }
	
	        /// <summary>
	        /// 单位
	        /// </summary>
	        [XmlElement("unit")]
	        public string Unit { get; set; }
}

    }
}
