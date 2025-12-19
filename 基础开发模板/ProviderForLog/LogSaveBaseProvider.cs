using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProviderForLog
{
    internal abstract class LogSaveBaseProvider:ILogSaveProvider
    {

        public bool SaveLog(LogEntity logEntity)
        {
            if(!this.IsSaveLogWithConfiguration(logEntity))
            {
                return false;
            }
            if (!this.ValidateLogEntity(logEntity))
            {
                return false;
            }
            this.FormatLogContent(logEntity);
            return DoSaveLog(logEntity);
        }

        public virtual bool IsSaveLogWithConfiguration(LogEntity logEntity)
        {
            string logType = ConfigurationManager.AppSettings["LogType"] ?? "";
            if (logEntity.Type.ToString().Equals(logType))
            {
                return true;
            }
            return false;
        }
        public virtual bool ValidateLogEntity(LogEntity logEntity)
        {
            if(logEntity==null||logEntity.Content==null)
            {
                return false;
            }
            return true;
        }
        public virtual void FormatLogContent(LogEntity logEntity)
        {
            
        }

        protected abstract bool DoSaveLog(LogEntity logEntity);
    }
}
