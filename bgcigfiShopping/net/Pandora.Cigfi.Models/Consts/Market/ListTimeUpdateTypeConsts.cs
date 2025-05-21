using System;
using System.Collections.Generic;
using System.Text;

namespace Pandora.Cigfi.Models.Consts.Market
{
    /// <summary>
    /// 交易对上架及下架时间设置类型
    /// </summary>
 public    class ListTimeUpdateTypeConsts
    {
        /// <summary>
        /// 设置上架时间
        /// </summary>
        public const string LISTTIME = "0";
        /// <summary>
        /// 设置下架时间
        /// </summary>
        public const string UNLISTTIME = "1";
        /// <summary>
        /// 创建时间->上架时间
        /// </summary>
        public const string CREATETIME_LISTTIME = "2";
        /// <summary>
        /// 行情最后一次更新时间_下架时间
        /// </summary>
        public const string TICKERTIME_UNLISTTIME = "3";
    }
}
