using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProviderForLog
{
    internal interface ILogSaveProvider
    {
        bool SaveLog(LogEntity logEntity);
    }
}
