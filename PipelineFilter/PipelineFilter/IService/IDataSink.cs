using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelineFilter.IService
{
    internal interface IDataSink<T> where T:IMessage
    {
        void Write(T message);
    }
}
