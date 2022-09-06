using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeVisitorDemo
{
    public class RaiseSalaryVisitor : IVisitor
    {
        public void Visit(IEmployee employee)
        {
            employee.Income += 1.1;
        }
    }
}
