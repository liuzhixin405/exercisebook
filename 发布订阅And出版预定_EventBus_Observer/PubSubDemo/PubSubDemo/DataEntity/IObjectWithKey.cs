using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSubDemo.DataEntity
{
    /// <summary>
    ///便于按照Key从持久层检索对象的抽象
    /// </summary>
    internal interface IObjectWithKey
    {
        string Key { get;}
    }
}
