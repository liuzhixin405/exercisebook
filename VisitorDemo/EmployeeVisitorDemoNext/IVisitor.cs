using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeVisitorDemoNext
{
    public interface IVisitor
    {
        void VisitManager(Manager manager);
        void VisitCharge(Charge charge);
    }
}
