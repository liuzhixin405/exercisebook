using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOC.Core
{
    [AttributeUsage(AttributeTargets.Constructor)]
    public class InjectionAttribute:Attribute
    {

    }
}
