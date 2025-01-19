using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// sosoBtc获取数据专用码表
    /// </summary>
    public partial class BqForSosobtc
    {
        public int Id { get; set; }
        /// <summary>
        /// 合约号
        /// </summary>
        public string TransactionCode { get; set; } = null!;
        /// <summary>
        /// sosobtc那边对应的坑位号
        /// </summary>
        public short PositionFlag { get; set; }
    }
}
