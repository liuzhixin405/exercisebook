using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Top.Api.Util;
using Top.Api;

namespace QimenCloud.Api.scenet33hglzv00.Request
{
    /// <summary>
    /// TOP API: wdgj.stockin.create
    /// </summary>
    public class WdgjStockinCreateRequest : BaseQimenCloudRequest<QimenCloud.Api.scenet33hglzv00.Response.WdgjStockinCreateResponse>
    {
        /// <summary>
        /// 根据format传入不同类型
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 长期有效的授权码，调用OpenApi时需要的主要验证
        /// </summary>
        public string Wdgjaccesstoken { get; set; }

        /// <summary>
        /// 创建应用接入时申请的appkey
        /// </summary>
        public string Wdgjappkey { get; set; }

        /// <summary>
        /// 数据返回值格式xml/json（默认xml）
        /// </summary>
        public string Wdgjformat { get; set; }

        /// <summary>
        /// 相关的业务接口名称编码
        /// </summary>
        public string Wdgjmethod { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        public string Wdgjsign { get; set; }

        /// <summary>
        /// 当前时间的Unix时间戳
        /// </summary>
        public string Wdgjtimestamp { get; set; }

        /// <summary>
        /// 协议版本号（1.1）
        /// </summary>
        public string Wdgjversions { get; set; }

        #region IQimenCloudRequest Members

        public override string GetApiName()
        {
            return "wdgj.stockin.create";
        }
        
        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("content", this.Content);
            parameters.Add("wdgjaccesstoken", this.Wdgjaccesstoken);
            parameters.Add("wdgjappkey", this.Wdgjappkey);
            parameters.Add("wdgjformat", this.Wdgjformat);
            parameters.Add("wdgjmethod", this.Wdgjmethod);
            parameters.Add("wdgjsign", this.Wdgjsign);
            parameters.Add("wdgjtimestamp", this.Wdgjtimestamp);
            parameters.Add("wdgjversions", this.Wdgjversions);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("content", this.Content);
            RequestValidator.ValidateRequired("wdgjaccesstoken", this.Wdgjaccesstoken);
            RequestValidator.ValidateRequired("wdgjappkey", this.Wdgjappkey);
            RequestValidator.ValidateRequired("wdgjformat", this.Wdgjformat);
            RequestValidator.ValidateRequired("wdgjmethod", this.Wdgjmethod);
            RequestValidator.ValidateRequired("wdgjsign", this.Wdgjsign);
            RequestValidator.ValidateRequired("wdgjtimestamp", this.Wdgjtimestamp);
            RequestValidator.ValidateRequired("wdgjversions", this.Wdgjversions);
        }

        #endregion
    }
}
