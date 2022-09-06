using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryMethod.DelegateFactory
{
    public interface IFactory<T>
    {
        T Create();
    }
}
