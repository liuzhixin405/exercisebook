using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Lzx.IOCDI.Framework.CustomContainer
{
    public interface ISelfContainer
    {
        void Register<TForm,TTo>(string shortName=null,object[] paraList = null,LifetimeType lifetimeType = LifetimeType.Transient) where TTo:TForm;
        void RegisterType(Type typeFrom, Type typeTo, LifetimeType lifetimeType = LifetimeType.Transient);
        TFrom Resolve<TFrom>(string shortName = null);
        object Resolve(Type type);

        ISelfContainer CreateChildContainer();
    }
}
