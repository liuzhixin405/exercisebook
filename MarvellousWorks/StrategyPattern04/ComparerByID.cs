using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyPattern04
{
    public class ComparerByID : IComparer<IBusinessObjct>
    {
        public int Compare(IBusinessObjct? x, IBusinessObjct? y)
        {
            if (x == null || y == null)
                throw new ArgumentNullException("x or y");
            return x.ID - y.ID;
        }
    }

}
