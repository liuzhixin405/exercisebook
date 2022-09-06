using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api;


namespace QimenCloud.Api.scenet33hglzv00.Response
{
    /// <summary>
    /// WdgjRequisitionListGetResponse.
    /// </summary>
    public class WdgjRequisitionListGetResponse : QimenCloudResponse
    {
        /// <summary>
        /// 调拨单信息
        /// </summary>
        [XmlArray("datalist")]
        [XmlArrayItem("datainfo")]
        public List<DatainfoDomain> Datalist { get; set; }

        /// <summary>
        /// 0 成功 -1 失败 1 部分失败
        /// </summary>
        [XmlElement("returncode")]
        public string Returncode { get; set; }

        /// <summary>
        /// failure
        /// </summary>
        [XmlElement("returnflag")]
        public string Returnflag { get; set; }

        /// <summary>
        /// 成功返回空，失败返回错误信息
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
	        /// 额外成本
	        /// </summary>
	        [XmlElement("extracost")]
	        public string Extracost { get; set; }
	
	        /// <summary>
	        /// 申请数量
	        /// </summary>
	        [XmlElement("goodscount")]
	        public string Goodscount { get; set; }
	
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
	        /// 入库数量
	        /// </summary>
	        [XmlElement("incount")]
	        public string Incount { get; set; }
	
	        /// <summary>
	        /// 出库数量
	        /// </summary>
	        [XmlElement("outcount")]
	        public string Outcount { get; set; }
	
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
	        /// 调拨单ID
	        /// </summary>
	        [XmlElement("billid")]
	        public string Billid { get; set; }
	
	        /// <summary>
	        /// 调拨单号
	        /// </summary>
	        [XmlElement("billno")]
	        public string Billno { get; set; }
	
	        /// <summary>
	        /// 创建时间
	        /// </summary>
	        [XmlElement("createtime")]
	        public string Createtime { get; set; }
	
	        /// <summary>
	        /// 状态（'0 待出库 1 待入库 2 已入库 3 待审核 4 被取消 5 已部分入库 7 被终止 8 待出库审核 9 待入库审核'）
	        /// </summary>
	        [XmlElement("curstatus")]
	        public string Curstatus { get; set; }
	
	        /// <summary>
	        /// 调拨类型
	        /// </summary>
	        [XmlElement("dbtype")]
	        public string Dbtype { get; set; }
	
	        /// <summary>
	        /// 调拨单中的货品信息
	        /// </summary>
	        [XmlArray("goodslist")]
	        [XmlArrayItem("goodsinfo")]
	        public List<GoodsinfoDomain> Goodslist { get; set; }
	
	        /// <summary>
	        /// 调入经办人
	        /// </summary>
	        [XmlElement("inchkoperator")]
	        public string Inchkoperator { get; set; }
	
	        /// <summary>
	        /// 调入时间
	        /// </summary>
	        [XmlElement("inchktime")]
	        public string Inchktime { get; set; }
	
	        /// <summary>
	        /// 调入备注
	        /// </summary>
	        [XmlElement("inremark")]
	        public string Inremark { get; set; }
	
	        /// <summary>
	        /// 物流费用
	        /// </summary>
	        [XmlElement("logisticfee")]
	        public string Logisticfee { get; set; }
	
	        /// <summary>
	        /// 物流ID
	        /// </summary>
	        [XmlElement("logisticid")]
	        public string Logisticid { get; set; }
	
	        /// <summary>
	        /// 物流公司
	        /// </summary>
	        [XmlElement("logisticname")]
	        public string Logisticname { get; set; }
	
	        /// <summary>
	        /// 物流单号
	        /// </summary>
	        [XmlElement("logisticno")]
	        public string Logisticno { get; set; }
	
	        /// <summary>
	        /// 物流类型
	        /// </summary>
	        [XmlElement("logistictype")]
	        public string Logistictype { get; set; }
	
	        /// <summary>
	        /// 调出经办人
	        /// </summary>
	        [XmlElement("outchkoperator")]
	        public string Outchkoperator { get; set; }
	
	        /// <summary>
	        /// 调出时间
	        /// </summary>
	        [XmlElement("outchktime")]
	        public string Outchktime { get; set; }
	
	        /// <summary>
	        /// 调出备注
	        /// </summary>
	        [XmlElement("outremark")]
	        public string Outremark { get; set; }
	
	        /// <summary>
	        /// 运单号
	        /// </summary>
	        [XmlElement("postid")]
	        public string Postid { get; set; }
	
	        /// <summary>
	        /// 备注
	        /// </summary>
	        [XmlElement("remark")]
	        public string Remark { get; set; }
	
	        /// <summary>
	        /// 创建人
	        /// </summary>
	        [XmlElement("requisitionoperator")]
	        public string Requisitionoperator { get; set; }
	
	        /// <summary>
	        /// 调入仓库id
	        /// </summary>
	        [XmlElement("warehousein")]
	        public string Warehousein { get; set; }
	
	        /// <summary>
	        /// 调入仓库名称
	        /// </summary>
	        [XmlElement("warehouseinname")]
	        public string Warehouseinname { get; set; }
	
	        /// <summary>
	        /// 调入仓库编码
	        /// </summary>
	        [XmlElement("warehouseinno")]
	        public string Warehouseinno { get; set; }
	
	        /// <summary>
	        /// 调出仓库id
	        /// </summary>
	        [XmlElement("warehouseout")]
	        public string Warehouseout { get; set; }
	
	        /// <summary>
	        /// 调出仓库名称
	        /// </summary>
	        [XmlElement("warehouseoutname")]
	        public string Warehouseoutname { get; set; }
	
	        /// <summary>
	        /// 调出仓库编码
	        /// </summary>
	        [XmlElement("warehouseoutno")]
	        public string Warehouseoutno { get; set; }
}

    }
}
