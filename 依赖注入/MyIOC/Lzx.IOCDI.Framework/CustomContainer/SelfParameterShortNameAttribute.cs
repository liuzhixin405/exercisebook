using System;
using System.Collections.Generic;
using System.Text;

namespace Lzx.IOCDI.Framework.CustomContainer
{
    public class SelfParameterShortNameAttribute : Attribute
    {
        public string ShortName { get; private set; }
        public SelfParameterShortNameAttribute(string shortName)
        {
            ShortName = shortName;
        }
    }
}
