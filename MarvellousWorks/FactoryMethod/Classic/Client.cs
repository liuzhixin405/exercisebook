using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryMethod.Classic
{
    public class Client
    {
        public void SomeMethod()
        {
            //获得抽象Factory的同时,与FactoryA产生依赖
            IFactory facrtory = new FactoryA();
            //后续操作仅以来抽象的IFactory和IProduct完成
            IProduct product = facrtory.Create();

        }
        private IFactory factory;
        public Client(IFactory factory)     //setter方式注入
        {
            if (factory == null) throw new ArgumentNullException("factory");
            this.factory = factory;
        }
        public void AnotherMethod()
        {
            IProduct product = factory.Create();
            //... ...
        }
    }
}
