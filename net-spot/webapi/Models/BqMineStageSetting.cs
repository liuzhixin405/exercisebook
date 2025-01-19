using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 平台挖矿各阶段参数设置
    /// </summary>
    public partial class BqMineStageSetting
    {
        public int Id { get; set; }
        /// <summary>
        /// 阶段编号
        /// </summary>
        public short Num { get; set; }
        /// <summary>
        /// 该阶段起始币量
        /// </summary>
        public decimal StageAmount { get; set; }
        /// <summary>
        /// 挖一个币所需要的手续费
        /// </summary>
        public decimal Rate { get; set; }
    }
}
