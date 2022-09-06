using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeVisitorDemoNext
{
    public interface IEmployee
    {
        string Name { get; set; }
        double Income { get; set; }
        int VacationDays { get; set; }

        void Accept(IVisitor visitor);
    }
}
