using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api;


namespace QimenCloud.Api.sceneh2s39v4h07.Response
{
    /// <summary>
    /// ApiSzsdPostxmlResponse.
    /// </summary>
    public class ApiSzsdPostxmlResponse : QimenCloudResponse
    {
        /// <summary>
        /// address
        /// </summary>
        [XmlElement("address")]
        public string Address { get; set; }

        /// <summary>
        /// name
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// rtnCode
        /// </summary>
        [XmlElement("rtnCode")]
        public string RtnCode { get; set; }

        /// <summary>
        /// rtnFlag
        /// </summary>
        [XmlElement("rtnFlag")]
        public string RtnFlag { get; set; }

        /// <summary>
        /// flag
        /// </summary>
        [XmlElement("rtnMessage")]
        public string RtnMessage { get; set; }

    }
}
