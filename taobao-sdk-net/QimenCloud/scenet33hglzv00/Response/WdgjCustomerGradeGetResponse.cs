using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api;


namespace QimenCloud.Api.scenet33hglzv00.Response
{
    /// <summary>
    /// WdgjCustomerGradeGetResponse.
    /// </summary>
    public class WdgjCustomerGradeGetResponse : QimenCloudResponse
    {
        /// <summary>
        /// 会员等级列表
        /// </summary>
        [XmlArray("datalist")]
        [XmlArrayItem("datainfo")]
        public List<DatainfoDomain> Datalist { get; set; }

        /// <summary>
        /// 二级错误码
        /// </summary>
        [XmlElement("returncode")]
        public string Returncode { get; set; }

        /// <summary>
        /// 响应失败出参判断条件
        /// </summary>
        [XmlElement("returnflag")]
        public string Returnflag { get; set; }

        /// <summary>
        /// 二级错误信息
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
	        /// 是否自动升级
	        /// </summary>
	        [XmlElement("bauto")]
	        public string Bauto { get; set; }
	
	        /// <summary>
	        /// 是否按积分升级
	        /// </summary>
	        [XmlElement("bscore")]
	        public string Bscore { get; set; }
	
	        /// <summary>
	        /// 价格折扣
	        /// </summary>
	        [XmlElement("dis")]
	        public string Dis { get; set; }
	
	        /// <summary>
	        /// 会员等级ID
	        /// </summary>
	        [XmlElement("membergradeid")]
	        public string Membergradeid { get; set; }
	
	        /// <summary>
	        /// 会员等级名
	        /// </summary>
	        [XmlElement("name")]
	        public string Name { get; set; }
	
	        /// <summary>
	        /// 执行价格名
	        /// </summary>
	        [XmlElement("price")]
	        public string Price { get; set; }
	
	        /// <summary>
	        /// 积分段起点
	        /// </summary>
	        [XmlElement("score1")]
	        public string Score1 { get; set; }
	
	        /// <summary>
	        /// 积分段终点
	        /// </summary>
	        [XmlElement("score2")]
	        public string Score2 { get; set; }
	
	        /// <summary>
	        /// 积分倍数
	        /// </summary>
	        [XmlElement("scorerate")]
	        public string Scorerate { get; set; }
}

    }
}
