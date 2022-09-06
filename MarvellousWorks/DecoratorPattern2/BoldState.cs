using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecoratorPattern2
{
    public class BoldState : IState
    {
        public bool IsBold;
        public bool Equals(IState newState)
        {
            if (newState == null) return false;
            return ((BoldState)newState).IsBold==IsBold;
        }
    }
}
