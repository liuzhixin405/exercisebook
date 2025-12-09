using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomObserver
{
    internal interface IObserver<T>
    {
        Task  UpdateAsync(T data);
    }
}
