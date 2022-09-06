using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeVisitorDemoNext
{
    public class IEmployeeCollection:List<IEmployee>
    {
        public virtual void Accept(IVisitor visitor)
        {
            foreach (IEmployee e in this)
            {
                e.Accept(visitor);
            }
        }
    }
}
