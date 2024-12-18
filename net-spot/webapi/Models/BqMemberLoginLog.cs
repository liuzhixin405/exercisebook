using System;
using System.Collections.Generic;

namespace webapi
{
    public partial class BqMemberLoginLog
    {
        public int Id { get; set; }
        public int FUserId { get; set; }
        public string? Ip { get; set; }
        public string? Note { get; set; }
        public int LoginTime { get; set; }
    }
}
