using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryMethod.DelegateFactory
{
    /// <summary>
    /// 委托本质上就是具体执行方法的抽象,相当与Product的角色
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    public delegate int CalculateHandler(params int[] items);
    class Calculator
    {
        /// <summary>
        /// 这个方法相当于Delegate Factory看到的Concrete Product
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public int Add(params int[] items)
        {
            int result = 0;
            foreach (var item in items)
            {
                result += item;
            }
            return result;
        }
    }
    /// <summary>
    /// Concrete Factory
    /// </summary>
    public class CalculateHandlerFactory : IFactory<CalculateHandler>
    {
        public CalculateHandler Create()
        {
            return (new Calculator()).Add;         
        }
    }
}
