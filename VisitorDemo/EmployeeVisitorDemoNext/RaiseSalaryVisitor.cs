using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeVisitorDemoNext
{
    public class RaiseSalaryVisitor : IVisitor
    {    
        public void VisitCharge(Charge charge)
        {
            charge.Income += 1000;
        }

        public void VisitManager(Manager manager)
        {
            manager.Income += 2000;
            ///额外分红
            manager.Divided += 10000;
        }
    }
}
