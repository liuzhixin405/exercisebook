using System;
using System.Collections.Generic;

namespace webapi
{
    public partial class BqManagerMenu
    {
        public int Id { get; set; }
        public string Link { get; set; } = null!;
        public string MenuName { get; set; } = null!;
        public string MenuNo { get; set; } = null!;
        public int State { get; set; }
    }
}
