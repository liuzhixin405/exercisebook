using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Top.Api.Util;
using Top.Api;

namespace QimenCloud.Api.scenet33hglzv00.Request
{
    /// <summary>
    /// TOP API: wdgj.customernosensitiveinfo.list.get
    /// </summary>
    public class WdgjCustomernosensitiveinfoListGetRequest : BaseQimenCloudRequest<QimenCloud.Api.scenet33hglzv00.Response.WdgjCustomernosensitiveinfoListGetResponse>
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public string Begintime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public string Endtime { get; set; }

        /// <summary>
        /// 客户档案更改日期，YYYY-MM-DD HH:mm:ss格式
        /// </summary>
        public string Modifydate { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        public string Pageno { get; set; }

        /// <summary>
        /// 每页条数，取值范围 1 ~ 100
        /// </summary>
        public string Pagesize { get; set; }

        /// <summary>
        /// 会员昵称
        /// </summary>
        public string Searchname { get; set; }

        /// <summary>
        /// 会员编号
        /// </summary>
        public string Searchno { get; set; }

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
        /// 接口名称
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
            return "wdgj.customernosensitiveinfo.list.get";
        }
        
        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("begintime", this.Begintime);
            parameters.Add("endtime", this.Endtime);
            parameters.Add("modifydate", this.Modifydate);
            parameters.Add("pageno", this.Pageno);
            parameters.Add("pagesize", this.Pagesize);
            parameters.Add("searchname", this.Searchname);
            parameters.Add("searchno", this.Searchno);
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
        }

        #endregion
    }
}
