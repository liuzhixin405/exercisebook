using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplatePattern.temp1
{
    /// <summary>
    /// 抽象的模板接口
    /// </summary>
    public interface IAbstract
    {
        int Quantity { get; }
        double Total { get; }
        double Average { get; }
    }
    /// <summary>
    /// 定义了算法梗概的抽象类型
    /// </summary>
    public abstract class AbstraceBase : IAbstract
    {
        public abstract int Quantity { get; }

        public abstract double Total { get; }

        /// <summary>
        /// 算法梗概
        /// </summary>
        public virtual double Average=> Total/Quantity; 
    }
}
