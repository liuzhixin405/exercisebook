using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeVisitorDemoNext
{
    public class ExtraVacationVisitor : IVisitor
    {

        public void VisitCharge(Charge charge)
        {
            charge.VacationDays += 2;
        }

        public void VisitManager(Manager manager)
        {
            manager.VacationDays += 1;
        }
    }
}
