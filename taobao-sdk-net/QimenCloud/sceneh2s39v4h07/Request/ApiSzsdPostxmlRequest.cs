using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Top.Api.Util;
using Top.Api;

namespace QimenCloud.Api.sceneh2s39v4h07.Request
{
    /// <summary>
    /// TOP API: api.szsd.postxml
    /// </summary>
    public class ApiSzsdPostxmlRequest : BaseQimenCloudRequest<QimenCloud.Api.sceneh2s39v4h07.Response.ApiSzsdPostxmlResponse>
    {
        #region IQimenCloudRequest Members

        public override string GetApiName()
        {
            return "api.szsd.postxml";
        }
        
        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
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
