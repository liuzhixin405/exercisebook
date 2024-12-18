using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 城市区域表，此表暂用不到
    /// </summary>
    public partial class BqAreaArea
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        /// <summary>
        /// 城市代码
        /// </summary>
        public string Citycode { get; set; } = null!;
    }
}
