using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api;


namespace QimenCloud.Api.scenet33hglzv00.Response
{
    /// <summary>
    /// WdgjGoodsListGetResponse.
    /// </summary>
    public class WdgjGoodsListGetResponse : QimenCloudResponse
    {
        /// <summary>
        /// 货品列表
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
/// SpecinfoDomain Data Structure.
/// </summary>
[Serializable]

public class SpecinfoDomain : TopObject
{
	        /// <summary>
	        /// 是否停用 True/False
	        /// </summary>
	        [XmlElement("bblockup")]
	        public string Bblockup { get; set; }
	
	        /// <summary>
	        /// 是否启用固定成本价 True/False
	        /// </summary>
	        [XmlElement("bfixcost")]
	        public string Bfixcost { get; set; }
	
	        /// <summary>
	        /// 固定成本价
	        /// </summary>
	        [XmlElement("fixcostprice")]
	        public string Fixcostprice { get; set; }
	
	        /// <summary>
	        /// 附加码
	        /// </summary>
	        [XmlElement("speccode")]
	        public string Speccode { get; set; }
	
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
	        /// 重量
	        /// </summary>
	        [XmlElement("weight")]
	        public string Weight { get; set; }
}

	/// <summary>
/// CrossborderDomain Data Structure.
/// </summary>
[Serializable]

public class CrossborderDomain : TopObject
{
	        /// <summary>
	        /// 检验检疫商品备案编号
	        /// </summary>
	        [XmlElement("goodnociq")]
	        public string Goodnociq { get; set; }
	
	        /// <summary>
	        /// 海关备案商品编号
	        /// </summary>
	        [XmlElement("goodnohs")]
	        public string Goodnohs { get; set; }
	
	        /// <summary>
	        /// 原产国，默认中国
	        /// </summary>
	        [XmlElement("goods_coo")]
	        public string GoodsCoo { get; set; }
	
	        /// <summary>
	        /// 海关备案商品编号（保税）
	        /// </summary>
	        [XmlElement("goodshsno_b")]
	        public string GoodshsnoB { get; set; }
	
	        /// <summary>
	        /// 海关备案商品编号（直邮)
	        /// </summary>
	        [XmlElement("goodshsno_m")]
	        public string GoodshsnoM { get; set; }
	
	        /// <summary>
	        /// 消费税税率
	        /// </summary>
	        [XmlElement("gst_taxrate")]
	        public string GstTaxrate { get; set; }
	
	        /// <summary>
	        /// h2010
	        /// </summary>
	        [XmlElement("h2010no")]
	        public string H2010no { get; set; }
	
	        /// <summary>
	        /// 关税税率
	        /// </summary>
	        [XmlElement("hs_taxrate")]
	        public string HsTaxrate { get; set; }
	
	        /// <summary>
	        /// 默认海关编号类型
	        /// </summary>
	        [XmlElement("hsnotype")]
	        public string Hsnotype { get; set; }
	
	        /// <summary>
	        /// 进口税税则号
	        /// </summary>
	        [XmlElement("hstaxno")]
	        public string Hstaxno { get; set; }
	
	        /// <summary>
	        /// 包装类型代码(检验检疫)默认其它
	        /// </summary>
	        [XmlElement("packtypeciq")]
	        public string Packtypeciq { get; set; }
	
	        /// <summary>
	        /// 包装类型代码(海关)默认其它
	        /// </summary>
	        [XmlElement("packtypehs")]
	        public string Packtypehs { get; set; }
	
	        /// <summary>
	        /// 行邮税号
	        /// </summary>
	        [XmlElement("taxid")]
	        public string Taxid { get; set; }
	
	        /// <summary>
	        /// 增值税税率
	        /// </summary>
	        [XmlElement("vat_taxrate")]
	        public string VatTaxrate { get; set; }
}

	/// <summary>
/// DatainfoDomain Data Structure.
/// </summary>
[Serializable]

public class DatainfoDomain : TopObject
{
	        /// <summary>
	        /// 条码
	        /// </summary>
	        [XmlElement("barcode")]
	        public string Barcode { get; set; }
	
	        /// <summary>
	        /// 是否赠品 True/False
	        /// </summary>
	        [XmlElement("bgift")]
	        public string Bgift { get; set; }
	
	        /// <summary>
	        /// 是否多规格 True/False
	        /// </summary>
	        [XmlElement("bmultispec")]
	        public string Bmultispec { get; set; }
	
	        /// <summary>
	        /// 积分
	        /// </summary>
	        [XmlElement("bonuspoints")]
	        public string Bonuspoints { get; set; }
	
	        /// <summary>
	        /// 品牌
	        /// </summary>
	        [XmlElement("brand")]
	        public string Brand { get; set; }
	
	        /// <summary>
	        /// 类别
	        /// </summary>
	        [XmlElement("classname")]
	        public string Classname { get; set; }
	
	        /// <summary>
	        /// 货品跨境信息
	        /// </summary>
	        [XmlElement("crossborderobj")]
	        public CrossborderDomain Crossborderobj { get; set; }
	
	        /// <summary>
	        /// 上架日期
	        /// </summary>
	        [XmlElement("days")]
	        public string Days { get; set; }
	
	        /// <summary>
	        /// 英文名
	        /// </summary>
	        [XmlElement("engname")]
	        public string Engname { get; set; }
	
	        /// <summary>
	        /// 高
	        /// </summary>
	        [XmlElement("goodsheight")]
	        public string Goodsheight { get; set; }
	
	        /// <summary>
	        /// 货品ID
	        /// </summary>
	        [XmlElement("goodsid")]
	        public string Goodsid { get; set; }
	
	        /// <summary>
	        /// 长
	        /// </summary>
	        [XmlElement("goodslen")]
	        public string Goodslen { get; set; }
	
	        /// <summary>
	        /// 品名
	        /// </summary>
	        [XmlElement("goodsname")]
	        public string Goodsname { get; set; }
	
	        /// <summary>
	        /// 别名
	        /// </summary>
	        [XmlElement("goodsname2")]
	        public string Goodsname2 { get; set; }
	
	        /// <summary>
	        /// 货品编号
	        /// </summary>
	        [XmlElement("goodsno")]
	        public string Goodsno { get; set; }
	
	        /// <summary>
	        /// 宽
	        /// </summary>
	        [XmlElement("goodswidth")]
	        public string Goodswidth { get; set; }
	
	        /// <summary>
	        /// 海关编码
	        /// </summary>
	        [XmlElement("hscode")]
	        public string Hscode { get; set; }
	
	        /// <summary>
	        /// 产地
	        /// </summary>
	        [XmlElement("origin")]
	        public string Origin { get; set; }
	
	        /// <summary>
	        /// 自定义价1
	        /// </summary>
	        [XmlElement("price1")]
	        public string Price1 { get; set; }
	
	        /// <summary>
	        /// 自定义价2
	        /// </summary>
	        [XmlElement("price2")]
	        public string Price2 { get; set; }
	
	        /// <summary>
	        /// 自定义价3
	        /// </summary>
	        [XmlElement("price3")]
	        public string Price3 { get; set; }
	
	        /// <summary>
	        /// 最低售价
	        /// </summary>
	        [XmlElement("pricebottom")]
	        public string Pricebottom { get; set; }
	
	        /// <summary>
	        /// 零售价
	        /// </summary>
	        [XmlElement("pricedetail")]
	        public string Pricedetail { get; set; }
	
	        /// <summary>
	        /// 会员价
	        /// </summary>
	        [XmlElement("pricemember")]
	        public string Pricemember { get; set; }
	
	        /// <summary>
	        /// 批发价
	        /// </summary>
	        [XmlElement("pricewholesale")]
	        public string Pricewholesale { get; set; }
	
	        /// <summary>
	        /// 采购员
	        /// </summary>
	        [XmlElement("purchaser")]
	        public string Purchaser { get; set; }
	
	        /// <summary>
	        /// 助记码
	        /// </summary>
	        [XmlElement("pycode")]
	        public string Pycode { get; set; }
	
	        /// <summary>
	        /// 备注
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
	        /// 规格列表
	        /// </summary>
	        [XmlArray("speclist")]
	        [XmlArrayItem("specinfo")]
	        public List<SpecinfoDomain> Speclist { get; set; }
	
	        /// <summary>
	        /// 单位
	        /// </summary>
	        [XmlElement("unit")]
	        public string Unit { get; set; }
}

    }
}
