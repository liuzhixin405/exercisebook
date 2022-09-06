using System;
using System.Collections.Generic;
using System.Text;

namespace Lzx.IOCDI.Framework.CustomContainer
{
    public class SelfContainerRegistModel
    {
        public Type TargetType { get; set; }
        public LifetimeType Lifetime { get; set; }
        public object SingletonInstance { get; set; }
    }
    public enum LifetimeType
    {
        Transient,
        Singleton,
        Scope,
        PerThread  //线程单例  外部可释放单例
    }
}
