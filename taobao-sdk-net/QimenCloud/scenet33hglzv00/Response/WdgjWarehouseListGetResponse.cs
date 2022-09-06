using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api;


namespace QimenCloud.Api.scenet33hglzv00.Response
{
    /// <summary>
    /// WdgjWarehouseListGetResponse.
    /// </summary>
    public class WdgjWarehouseListGetResponse : QimenCloudResponse
    {
        /// <summary>
        /// 仓库列表
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
	        /// 允许自动添加货品 0 否 1 是
	        /// </summary>
	        [XmlElement("bautoaddgoods")]
	        public string Bautoaddgoods { get; set; }
	
	        /// <summary>
	        /// 是否停用 True/False
	        /// </summary>
	        [XmlElement("bblockup")]
	        public string Bblockup { get; set; }
	
	        /// <summary>
	        /// 允许负库存出库 0 否 1 是
	        /// </summary>
	        [XmlElement("bnegativestock")]
	        public string Bnegativestock { get; set; }
	
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
	        /// 联系人
	        /// </summary>
	        [XmlElement("linkman")]
	        public string Linkman { get; set; }
	
	        /// <summary>
	        /// 州省
	        /// </summary>
	        [XmlElement("province")]
	        public string Province { get; set; }
	
	        /// <summary>
	        /// 电话
	        /// </summary>
	        [XmlElement("tel")]
	        public string Tel { get; set; }
	
	        /// <summary>
	        /// 区县
	        /// </summary>
	        [XmlElement("town")]
	        public string Town { get; set; }
	
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
	        /// 仓库编码
	        /// </summary>
	        [XmlElement("warehouseno")]
	        public string Warehouseno { get; set; }
	
	        /// <summary>
	        /// 仓库类别 0 本部仓库 1 委外仓库 2 残次仓库 3 保税仓 4 委外残次品仓
	        /// </summary>
	        [XmlElement("warehousetype")]
	        public string Warehousetype { get; set; }
	
	        /// <summary>
	        /// 邮编
	        /// </summary>
	        [XmlElement("zip")]
	        public string Zip { get; set; }
}

    }
}
