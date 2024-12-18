using System;
using System.Collections.Generic;

namespace webapi
{
    public partial class BqMinePersonalAmount
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public int Uid { get; set; }
        /// <summary>
        /// 用户已挖数量
        /// </summary>
        public decimal MinedAmount { get; set; }
    }
}
