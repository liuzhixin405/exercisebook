using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDCM.Contract.Core.Extension
{
    public class AjaxResult
    {
        public bool Success { get; set; } = true;
        public int ErrorCode { get; set; } = -1;
        public string Msg { get; set; }
    }
}
