using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api;


namespace QimenCloud.Api.scenet33hglzv00.Response
{
    /// <summary>
    /// WdgjProviderListGetResponse.
    /// </summary>
    public class WdgjProviderListGetResponse : QimenCloudResponse
    {
        /// <summary>
        /// 供应商列表
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
/// DatainfoDomain Data Structure.
/// </summary>
[Serializable]

public class DatainfoDomain : TopObject
{
	        /// <summary>
	        /// 详细地址
	        /// </summary>
	        [XmlElement("adr")]
	        public string Adr { get; set; }
	
	        /// <summary>
	        /// 银行卡号
	        /// </summary>
	        [XmlElement("bankacct")]
	        public string Bankacct { get; set; }
	
	        /// <summary>
	        /// 是否停用 True/False
	        /// </summary>
	        [XmlElement("bblockup")]
	        public string Bblockup { get; set; }
	
	        /// <summary>
	        /// 区市
	        /// </summary>
	        [XmlElement("city")]
	        public string City { get; set; }
	
	        /// <summary>
	        /// 国家
	        /// </summary>
	        [XmlElement("country")]
	        public string Country { get; set; }
	
	        /// <summary>
	        /// 邮箱
	        /// </summary>
	        [XmlElement("email")]
	        public string Email { get; set; }
	
	        /// <summary>
	        /// 供应商商号
	        /// </summary>
	        [XmlElement("esno")]
	        public string Esno { get; set; }
	
	        /// <summary>
	        /// 传真
	        /// </summary>
	        [XmlElement("fax")]
	        public string Fax { get; set; }
	
	        /// <summary>
	        /// 其他IM
	        /// </summary>
	        [XmlElement("im")]
	        public string Im { get; set; }
	
	        /// <summary>
	        /// 联系人
	        /// </summary>
	        [XmlElement("linkman")]
	        public string Linkman { get; set; }
	
	        /// <summary>
	        /// 排序
	        /// </summary>
	        [XmlElement("orderpos")]
	        public string Orderpos { get; set; }
	
	        /// <summary>
	        /// 日产能
	        /// </summary>
	        [XmlElement("power")]
	        public string Power { get; set; }
	
	        /// <summary>
	        /// 产能饱和数
	        /// </summary>
	        [XmlElement("powerrate")]
	        public string Powerrate { get; set; }
	
	        /// <summary>
	        /// 经营范围
	        /// </summary>
	        [XmlElement("products")]
	        public string Products { get; set; }
	
	        /// <summary>
	        /// 别名
	        /// </summary>
	        [XmlElement("provideralias")]
	        public string Provideralias { get; set; }
	
	        /// <summary>
	        /// 供应商ID
	        /// </summary>
	        [XmlElement("providerid")]
	        public string Providerid { get; set; }
	
	        /// <summary>
	        /// 供应商名称
	        /// </summary>
	        [XmlElement("providername")]
	        public string Providername { get; set; }
	
	        /// <summary>
	        /// 供应商编号
	        /// </summary>
	        [XmlElement("providerno")]
	        public string Providerno { get; set; }
	
	        /// <summary>
	        /// 州省
	        /// </summary>
	        [XmlElement("province")]
	        public string Province { get; set; }
	
	        /// <summary>
	        /// 助记码
	        /// </summary>
	        [XmlElement("pycode")]
	        public string Pycode { get; set; }
	
	        /// <summary>
	        /// QQ
	        /// </summary>
	        [XmlElement("qq")]
	        public string Qq { get; set; }
	
	        /// <summary>
	        /// 备注
	        /// </summary>
	        [XmlElement("remark")]
	        public string Remark { get; set; }
	
	        /// <summary>
	        /// 电话
	        /// </summary>
	        [XmlElement("tel")]
	        public string Tel { get; set; }
	
	        /// <summary>
	        /// 区镇
	        /// </summary>
	        [XmlElement("town")]
	        public string Town { get; set; }
	
	        /// <summary>
	        /// 网站
	        /// </summary>
	        [XmlElement("website")]
	        public string Website { get; set; }
	
	        /// <summary>
	        /// 邮编
	        /// </summary>
	        [XmlElement("zip")]
	        public string Zip { get; set; }
}

    }
}
