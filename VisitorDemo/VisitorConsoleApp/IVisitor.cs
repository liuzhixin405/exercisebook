using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorConsoleApp
{
    internal interface IVisitor
    {
        void Visit(IEmployee employee);
    }
}
