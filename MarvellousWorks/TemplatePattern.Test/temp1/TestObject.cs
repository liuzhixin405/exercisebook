using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplatePattern.temp1;

namespace TemplatePattern.Test.temp1
{
    /// <summary>
    /// 具体类型
    /// </summary>
    public class ArrayData : AbstraceBase
    {
        protected double[] data = new double[3] { 1.1, 2.2, 3.3 };
        public override int Quantity => data.Length;

        public override double Total => data.Sum();
    }
    /// <summary>
    /// 具体类型
    /// </summary>
    public class ListData : AbstraceBase
    {
        protected IList<double> data = new List<double>();
        public ListData()
        {
            data.Add(1.1);
            data.Add(2.2);
            data.Add(3.3);
        }

        public override int Quantity => data.Count;

        public override double Total => data.Sum();
    }
}
