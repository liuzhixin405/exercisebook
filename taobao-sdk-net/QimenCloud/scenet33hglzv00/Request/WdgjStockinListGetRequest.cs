using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Top.Api.Util;
using Top.Api;

namespace QimenCloud.Api.scenet33hglzv00.Request
{
    /// <summary>
    /// TOP API: wdgj.stockin.list.get
    /// </summary>
    public class WdgjStockinListGetRequest : BaseQimenCloudRequest<QimenCloud.Api.scenet33hglzv00.Response.WdgjStockinListGetResponse>
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
        /// 单据类型（'2 （采购入库，采购入库冲销） 3退货入库 5库存盘点 9调拨入库 10 生产入库'）
        /// </summary>
        public string Operationtype { get; set; }

        /// <summary>
        /// 0 API导入商品 0 其它入库 0 其它入库冲销 0 组装拆卸 0 组装拆卸冲销 2 采购入库 2 采购入库冲销 3 退货入库 3 退货入库冲销 5 库存盘点 5 库存盘点冲销 9 调拨入库 9 调拨入库冲销 10 生产入库 10 生产退料入库 12 其它入库 12 其它入库冲销
        /// </summary>
        public string Ordertype { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        public string Pageno { get; set; }

        /// <summary>
        /// 每页条数，取值范围 1 ~ 100
        /// </summary>
        public string Pagesize { get; set; }

        /// <summary>
        /// 入库单号
        /// </summary>
        public string Searchno { get; set; }

        /// <summary>
        /// 0 待审核 1 已审核 2 已取消'
        /// </summary>
        public string Searchstatus { get; set; }

        /// <summary>
        /// 0查询未归档数据，1查询归档数据（不填或者填写其他值默认查询未归档数据）
        /// </summary>
        public string Searchtype { get; set; }

        /// <summary>
        /// 时间类型（0：表示登记时间，1：表示审核时间）
        /// </summary>
        public string Timetype { get; set; }

        /// <summary>
        /// 仓库编号
        /// </summary>
        public string Warehouseno { get; set; }

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
            return "wdgj.stockin.list.get";
        }
        
        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("begintime", this.Begintime);
            parameters.Add("endtime", this.Endtime);
            parameters.Add("operationtype", this.Operationtype);
            parameters.Add("ordertype", this.Ordertype);
            parameters.Add("pageno", this.Pageno);
            parameters.Add("pagesize", this.Pagesize);
            parameters.Add("searchno", this.Searchno);
            parameters.Add("searchstatus", this.Searchstatus);
            parameters.Add("searchtype", this.Searchtype);
            parameters.Add("timetype", this.Timetype);
            parameters.Add("warehouseno", this.Warehouseno);
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
            RequestValidator.ValidateRequired("begintime", this.Begintime);
            RequestValidator.ValidateRequired("endtime", this.Endtime);
            RequestValidator.ValidateRequired("pageno", this.Pageno);
            RequestValidator.ValidateRequired("pagesize", this.Pagesize);
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
