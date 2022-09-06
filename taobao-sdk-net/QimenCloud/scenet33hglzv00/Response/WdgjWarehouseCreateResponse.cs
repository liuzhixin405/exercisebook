using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api;


namespace QimenCloud.Api.scenet33hglzv00.Response
{
    /// <summary>
    /// WdgjWarehouseCreateResponse.
    /// </summary>
    public class WdgjWarehouseCreateResponse : QimenCloudResponse
    {
        /// <summary>
        /// datalist
        /// </summary>
        [XmlArray("datalist")]
        [XmlArrayItem("datainfo")]
        public List<DatainfoDomain> Datalist { get; set; }

        /// <summary>
        /// returncode
        /// </summary>
        [XmlElement("returncode")]
        public string Returncode { get; set; }

        /// <summary>
        /// failure
        /// </summary>
        [XmlElement("returnflag")]
        public string Returnflag { get; set; }

        /// <summary>
        /// returninfo
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
	        /// errmsg
	        /// </summary>
	        [XmlElement("errmsg")]
	        public string Errmsg { get; set; }
	
	        /// <summary>
	        /// warehousename
	        /// </summary>
	        [XmlElement("warehousename")]
	        public string Warehousename { get; set; }
	
	        /// <summary>
	        /// warehouseno
	        /// </summary>
	        [XmlElement("warehouseno")]
	        public string Warehouseno { get; set; }
}

    }
}
