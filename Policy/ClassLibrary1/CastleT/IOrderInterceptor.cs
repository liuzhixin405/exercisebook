using System;
using System.Collections.Generic;
using System.Text;

namespace Castle.Test.CastleT
{
    public interface IOrderService
    {
        public void MethodInterceptor();
        public void MethodNoInterceptor();
    }
}
