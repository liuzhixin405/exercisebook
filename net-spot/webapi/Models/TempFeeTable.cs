using System;
using System.Collections.Generic;

namespace webapi
{
    public partial class TempFeeTable
    {
        public int Id { get; set; }
        public int FUserId { get; set; }
        public decimal Fee { get; set; }
    }
}
