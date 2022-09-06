using System;
using System.Collections.Generic;
using System.Text;

namespace Castle.Test.CastleT
{
    public class OrderServiceTwo : IOrderService
    {
        public virtual void MethodInterceptor()
        {
            Console.WriteLine(" 可以加工的方法 ");
        }

        public void MethodNoInterceptor()
        {
            Console.WriteLine(" 不可以实现Aop的方法 ");
        }
    }
}
