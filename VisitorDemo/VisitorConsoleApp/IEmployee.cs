using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorConsoleApp
{
    internal interface IEmployee
    {
        string Name { get; }
        double Income { get; set; }
        int VacationDays { get; set; }

        void Accept(IVisitor visitor);
    }
}
