using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeVisitorDemo
{
    public class ExtraVacationVisitor : IVisitor
    {
        public void Visit(IEmployee employee)
        {
            employee.VacationDays += 1;
        }
    }
}
