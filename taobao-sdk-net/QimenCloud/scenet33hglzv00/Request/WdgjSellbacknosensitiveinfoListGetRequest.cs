using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Top.Api.Util;
using Top.Api;

namespace QimenCloud.Api.scenet33hglzv00.Request
{
    /// <summary>
    /// TOP API: wdgj.sellbacknosensitiveinfo.list.get
    /// </summary>
    public class WdgjSellbacknosensitiveinfoListGetRequest : BaseQimenCloudRequest<QimenCloud.Api.scenet33hglzv00.Response.WdgjSellbacknosensitiveinfoListGetResponse>
    {
        /// <summary>
        /// 查询开始时间 YYYY-MM-DD HH:mm:ss
        /// </summary>
        public string Begintime { get; set; }

        /// <summary>
        /// 查询结束时间 YYYY-MM-DD HH:mm:ss
        /// </summary>
        public string Endtime { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        public string Pageno { get; set; }

        /// <summary>
        /// 每页条数，取值范围 1 ~ 100
        /// </summary>
        public string Pagesize { get; set; }

        /// <summary>
        /// 关联单号（管家订单号）
        /// </summary>
        public string Relationno { get; set; }

        /// <summary>
        /// 退换单号
        /// </summary>
        public string Searchno { get; set; }

        /// <summary>
        /// 0 待收货 1 待结算 2 被取消 3 已完成 4 待审核 默认所有
        /// </summary>
        public string Searchstatus { get; set; }

        /// <summary>
        /// 时间类型（1：表示登记时间，2：表示收货时间, 3：表示结算时间 不填或者填写其他值默认为登记时间）
        /// </summary>
        public string Timetype { get; set; }

        /// <summary>
        /// 长期有效的授权码，调用OpenApi时需要的主要验证
        /// </summary>
        public string Wdgjaccesstoken { get; set; }

        /// <summary>
        /// 创建应用接入时申请的appkey
        /// </summary>
        public string Wdgjappkey { get; set; }

        /// <summary>
        /// 数据返回值格式xml/json
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
        /// 协议版本号
        /// </summary>
        public string Wdgjversions { get; set; }

        #region IQimenCloudRequest Members

        public override string GetApiName()
        {
            return "wdgj.sellbacknosensitiveinfo.list.get";
        }
        
        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("begintime", this.Begintime);
            parameters.Add("endtime", this.Endtime);
            parameters.Add("pageno", this.Pageno);
            parameters.Add("pagesize", this.Pagesize);
            parameters.Add("relationno", this.Relationno);
            parameters.Add("searchno", this.Searchno);
            parameters.Add("searchstatus", this.Searchstatus);
            parameters.Add("timetype", this.Timetype);
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
