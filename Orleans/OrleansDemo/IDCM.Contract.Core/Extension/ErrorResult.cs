using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDCM.Contract.Core.Extension
{
    public class ErrorResult:AjaxResult
    {
        public ErrorResult(string msg="操作失败",int errorCode=0)
        {
            Msg = msg;
            Success = false;
            ErrorCode = errorCode;
        }
    }
}
