using System;
using System.Collections.Generic;
using System.Text;

namespace Lzx.IOCDI.Framework.CustomContainer
{
    [AttributeUsage(AttributeTargets.Constructor)]
    public class SelfConstructorAttribute:Attribute
    {
    }
}
