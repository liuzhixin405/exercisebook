using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api;


namespace QimenCloud.Api.scenet33hglzv00.Response
{
    /// <summary>
    /// WdgjCustomernosensitiveinfoListGetResponse.
    /// </summary>
    public class WdgjCustomernosensitiveinfoListGetResponse : QimenCloudResponse
    {
        /// <summary>
        /// 会员列表
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
	        /// 地址
	        /// </summary>
	        [XmlElement("adr")]
	        public string Adr { get; set; }
	
	        /// <summary>
	        /// 欠款额度
	        /// </summary>
	        [XmlElement("arrearagevalue")]
	        public string Arrearagevalue { get; set; }
	
	        /// <summary>
	        /// 退货次数
	        /// </summary>
	        [XmlElement("backcount")]
	        public string Backcount { get; set; }
	
	        /// <summary>
	        /// 客户预存款
	        /// </summary>
	        [XmlElement("balance")]
	        public string Balance { get; set; }
	
	        /// <summary>
	        /// 生日
	        /// </summary>
	        [XmlElement("birthday")]
	        public string Birthday { get; set; }
	
	        /// <summary>
	        /// 是否黑名单True 是 False 否
	        /// </summary>
	        [XmlElement("blacklist")]
	        public string Blacklist { get; set; }
	
	        /// <summary>
	        /// 是否不发送短信，邮件 True 是 False 否
	        /// </summary>
	        [XmlElement("brefuse")]
	        public string Brefuse { get; set; }
	
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
	        /// 系统编码
	        /// </summary>
	        [XmlElement("customerid")]
	        public string Customerid { get; set; }
	
	        /// <summary>
	        /// 姓名
	        /// </summary>
	        [XmlElement("customername")]
	        public string Customername { get; set; }
	
	        /// <summary>
	        /// 邮箱
	        /// </summary>
	        [XmlElement("email")]
	        public string Email { get; set; }
	
	        /// <summary>
	        /// 印象
	        /// </summary>
	        [XmlElement("feelings")]
	        public string Feelings { get; set; }
	
	        /// <summary>
	        /// 标记
	        /// </summary>
	        [XmlElement("flagid")]
	        public string Flagid { get; set; }
	
	        /// <summary>
	        /// 消费金额
	        /// </summary>
	        [XmlElement("goodstotal")]
	        public string Goodstotal { get; set; }
	
	        /// <summary>
	        /// 提醒
	        /// </summary>
	        [XmlElement("hintremark")]
	        public string Hintremark { get; set; }
	
	        /// <summary>
	        /// 是否分销商True 是 False 否
	        /// </summary>
	        [XmlElement("isfxs")]
	        public string Isfxs { get; set; }
	
	        /// <summary>
	        /// 会员修改时间
	        /// </summary>
	        [XmlElement("lasteditdate")]
	        public string Lasteditdate { get; set; }
	
	        /// <summary>
	        /// 默认物流id
	        /// </summary>
	        [XmlElement("logisticid")]
	        public string Logisticid { get; set; }
	
	        /// <summary>
	        /// 默认物流名
	        /// </summary>
	        [XmlElement("logisticname")]
	        public string Logisticname { get; set; }
	
	        /// <summary>
	        /// 默认物流编号
	        /// </summary>
	        [XmlElement("logisticno")]
	        public string Logisticno { get; set; }
	
	        /// <summary>
	        /// 会员等级id
	        /// </summary>
	        [XmlElement("membergradeid")]
	        public string Membergradeid { get; set; }
	
	        /// <summary>
	        /// 会员编号
	        /// </summary>
	        [XmlElement("memberid")]
	        public string Memberid { get; set; }
	
	        /// <summary>
	        /// 网名
	        /// </summary>
	        [XmlElement("nickname")]
	        public string Nickname { get; set; }
	
	        /// <summary>
	        /// 购买力
	        /// </summary>
	        [XmlElement("power")]
	        public string Power { get; set; }
	
	        /// <summary>
	        /// 省
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
	        /// 推荐人id
	        /// </summary>
	        [XmlElement("recommenderid")]
	        public string Recommenderid { get; set; }
	
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
	        /// 消费积分
	        /// </summary>
	        [XmlElement("score")]
	        public string Score { get; set; }
	
	        /// <summary>
	        /// 业务员id
	        /// </summary>
	        [XmlElement("sellerid")]
	        public string Sellerid { get; set; }
	
	        /// <summary>
	        /// 价格敏感
	        /// </summary>
	        [XmlElement("sensitive")]
	        public string Sensitive { get; set; }
	
	        /// <summary>
	        /// 性别
	        /// </summary>
	        [XmlElement("sex")]
	        public string Sex { get; set; }
	
	        /// <summary>
	        /// 所属店铺id
	        /// </summary>
	        [XmlElement("shopid")]
	        public string Shopid { get; set; }
	
	        /// <summary>
	        /// 所属店铺
	        /// </summary>
	        [XmlElement("shopname")]
	        public string Shopname { get; set; }
	
	        /// <summary>
	        /// 所属店铺编码
	        /// </summary>
	        [XmlElement("shopno")]
	        public string Shopno { get; set; }
	
	        /// <summary>
	        /// 风格
	        /// </summary>
	        [XmlElement("style")]
	        public string Style { get; set; }
	
	        /// <summary>
	        /// 联系电话
	        /// </summary>
	        [XmlElement("tel")]
	        public string Tel { get; set; }
	
	        /// <summary>
	        /// 县
	        /// </summary>
	        [XmlElement("town")]
	        public string Town { get; set; }
	
	        /// <summary>
	        /// 购物次数
	        /// </summary>
	        [XmlElement("tradecount")]
	        public string Tradecount { get; set; }
	
	        /// <summary>
	        /// 购物金额合计
	        /// </summary>
	        [XmlElement("tradetotal")]
	        public string Tradetotal { get; set; }
	
	        /// <summary>
	        /// 已用积分
	        /// </summary>
	        [XmlElement("usedscore")]
	        public string Usedscore { get; set; }
	
	        /// <summary>
	        /// 邮编
	        /// </summary>
	        [XmlElement("zip")]
	        public string Zip { get; set; }
}

    }
}
