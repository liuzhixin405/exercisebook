using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 积分日志表(网站可以手工添加积分/用户自行购买积分/进行一些资料完善奖励/交易奖励/其它奖励)
    /// </summary>
    public partial class BqCommonCreditLog
    {
        public uint CreditLogId { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public uint FUserId { get; set; }
        /// <summary>
        /// 积分类别(后台分析统计)(1.注册/2.登录/3.绑定手机/4.绑定Google/5.充值/6.交易/7.购买/8.奖励)
        /// </summary>
        public byte CreditTypeId { get; set; }
        /// <summary>
        /// 积分数量
        /// </summary>
        public ulong Credits { get; set; }
        /// <summary>
        /// 积分标题
        /// </summary>
        public string Title { get; set; } = null!;
        /// <summary>
        /// 积分时间(unix格式，排序快)
        /// </summary>
        public uint Dateline { get; set; }
    }
}
