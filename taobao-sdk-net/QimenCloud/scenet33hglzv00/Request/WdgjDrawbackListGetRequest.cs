using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Top.Api.Util;
using Top.Api;

namespace QimenCloud.Api.scenet33hglzv00.Request
{
    /// <summary>
    /// TOP API: wdgj.drawback.list.get
    /// </summary>
    public class WdgjDrawbackListGetRequest : BaseQimenCloudRequest<QimenCloud.Api.scenet33hglzv00.Response.WdgjDrawbackListGetResponse>
    {
        /// <summary>
        /// 查询开始时间格式：yyyy-MM-dd hh:mm:ss
        /// </summary>
        public string Begintime { get; set; }

        /// <summary>
        /// 查询结束时间格式：yyyy-MM-dd hh:mm:ss
        /// </summary>
        public string Endtime { get; set; }

        /// <summary>
        /// 查询的开始页码，必须大于0
        /// </summary>
        public string Pageno { get; set; }

        /// <summary>
        /// 查询的页记录数，取值1~100
        /// </summary>
        public string Pagesize { get; set; }

        /// <summary>
        /// 关联的订单号
        /// </summary>
        public string Searchno { get; set; }

        /// <summary>
        /// 退款状态 0已结算完成 1待收支审核 2应付款待结算 3收支结算完成 4收支已冲销 5取消收支 6应付结算完成 7应付部分结算 8取消应付 9应付已冲销
        /// </summary>
        public string Searchstatus { get; set; }

        /// <summary>
        /// 店铺名称
        /// </summary>
        public string Shopname { get; set; }

        /// <summary>
        /// 时间类型，配合 begintime 和 endtime使用
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
            return "wdgj.drawback.list.get";
        }
        
        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("begintime", this.Begintime);
            parameters.Add("endtime", this.Endtime);
            parameters.Add("pageno", this.Pageno);
            parameters.Add("pagesize", this.Pagesize);
            parameters.Add("searchno", this.Searchno);
            parameters.Add("searchstatus", this.Searchstatus);
            parameters.Add("shopname", this.Shopname);
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
