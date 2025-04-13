using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace visitordesign
{
    internal interface IVisitor
    {
        void VisitEmployee(IEmployee employee);
        void VisitManager(Manager manager);
    }
}
