using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObserverTwo
{
    internal interface IUpdatebleObject
    {
        int Data { get; }
        void Update(int newData);
    }

}
