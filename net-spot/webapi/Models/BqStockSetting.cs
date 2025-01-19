using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 股指参数设置
    /// </summary>
    public partial class BqStockSetting
    {
        public uint Id { get; set; }
        /// <summary>
        /// 上午开市时间
        /// </summary>
        public int AmBeginTime { get; set; }
        /// <summary>
        /// 上午闭市时间
        /// </summary>
        public int AmEndTime { get; set; }
        /// <summary>
        /// 下午开市时间
        /// </summary>
        public int PmBeginTime { get; set; }
        /// <summary>
        /// 下午闭市时间
        /// </summary>
        public int PmEndTime { get; set; }
        /// <summary>
        /// 假期判断api
        /// </summary>
        public string HolidayUrl { get; set; } = null!;
        /// <summary>
        /// 是否启用假期判断
        /// </summary>
        public ulong HolidayUrlUsing { get; set; }
        /// <summary>
        /// 合约号id
        /// </summary>
        public short TransactionId { get; set; }
        /// <summary>
        /// 周一是否为交易日（默认是）
        /// </summary>
        public bool MonState { get; set; }
        /// <summary>
        /// 周一交易时间,逗号&quot;,&quot;隔开,&quot;-&quot;连接开始和结束
        /// </summary>
        public string MonTradeTime { get; set; } = null!;
        /// <summary>
        /// 周二是否为交易日（默认是）
        /// </summary>
        public bool TueState { get; set; }
        /// <summary>
        /// 周二交易时间,逗号&quot;,&quot;隔开,&quot;-&quot;连接开始和结束
        /// </summary>
        public string TueTradeTime { get; set; } = null!;
        /// <summary>
        /// 周三是否为交易日（默认是）
        /// </summary>
        public bool WedState { get; set; }
        /// <summary>
        /// 周三交易时间,逗号&quot;,&quot;隔开,&quot;-&quot;连接开始和结束
        /// </summary>
        public string WedTradeTime { get; set; } = null!;
        /// <summary>
        /// 周四是否为交易日（默认是）
        /// </summary>
        public bool ThuState { get; set; }
        /// <summary>
        /// 周四交易时间,逗号&quot;,&quot;隔开,&quot;-&quot;连接开始和结束
        /// </summary>
        public string ThuTradeTime { get; set; } = null!;
        /// <summary>
        /// 周五是否为交易日（默认是）
        /// </summary>
        public bool FriState { get; set; }
        /// <summary>
        /// 周五交易时间,逗号&quot;,&quot;隔开,&quot;-&quot;连接开始和结束
        /// </summary>
        public string FriTradeTime { get; set; } = null!;
        /// <summary>
        /// 周六是否为交易日（默认是）
        /// </summary>
        public bool SatState { get; set; }
        /// <summary>
        /// 周六交易时间,逗号&quot;,&quot;隔开,&quot;-&quot;连接开始和结束
        /// </summary>
        public string SatTradeTime { get; set; } = null!;
        /// <summary>
        /// 周日是否为交易日（默认是）
        /// </summary>
        public bool SunState { get; set; }
        /// <summary>
        /// 周日交易时间,逗号&quot;,&quot;隔开,&quot;-&quot;连接开始和结束
        /// </summary>
        public string SunTradeTime { get; set; } = null!;
    }
}
