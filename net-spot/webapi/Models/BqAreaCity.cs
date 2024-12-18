using System;
using System.Collections.Generic;

namespace webapi
{
    /// <summary>
    /// 区域城市表
    /// </summary>
    public partial class BqAreaCity
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Provincecode { get; set; } = null!;
    }
}
