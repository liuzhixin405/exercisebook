using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorConsoleApp
{
    internal class EmployeeCollection:List<IEmployee>
    {
       public virtual void Accept(IVisitor visitor)
        {
            foreach (IEmployee employee in this)
            {
                employee.Accept(visitor);
            }
        }
    }
}
