using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProviderForLog
{
    internal class LogSaveLocalhostProvider : LogSaveBaseProvider
    {
        protected override bool DoSaveLog(LogEntity logEntity)
        {
            Console.WriteLine($"Log saved to localhost: {logEntity.Content.Message}");
            return true;
        }

        public override bool ValidateLogEntity(LogEntity logEntity)
        {
            if(base.ValidateLogEntity(logEntity))
            {
                if(string.IsNullOrEmpty(logEntity.Content?.LogTrackInfo))
                {
                    return false;
                }   
            }
            return true;
        }

        public override void FormatLogContent(LogEntity logEntity)
        {
            logEntity.Content.Message = logEntity.Content.Message.Replace("\\", "--");
        }
    }
}
