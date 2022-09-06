using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Top.Api.Util;
using Top.Api;

namespace QimenCloud.Api.scenet33hglzv00.Request
{
    /// <summary>
    /// TOP API: wdgj.open.api.qimen
    /// </summary>
    public class WdgjOpenApiQimenRequest : BaseQimenCloudRequest<QimenCloud.Api.scenet33hglzv00.Response.WdgjOpenApiQimenResponse>
    {
        /// <summary>
        /// compara
        /// </summary>
        public string Compara { get; set; }

        #region IQimenCloudRequest Members

        public override string GetApiName()
        {
            return "wdgj.open.api.qimen";
        }
        
        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("compara", this.Compara);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
        }

        #endregion
    }
}
