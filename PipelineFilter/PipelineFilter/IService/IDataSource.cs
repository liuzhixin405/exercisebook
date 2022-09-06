using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelineFilter.IService
{
    internal interface IDataSource<T> where T:IMessage
    {
        T Read();
    }
}
