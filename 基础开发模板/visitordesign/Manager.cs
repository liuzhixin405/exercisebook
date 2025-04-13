using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace visitordesign
{
    internal class Manager : Employee
    {
        private string department;
        public string Department { get=> department; }
        public Manager(string name, double income, int vacationDays, string department) : base(name, income, vacationDays)
        {
            this.department = department;
        }

        public override void Accept(IVisitor visitor)
        {
            visitor.VisitManager(this);
        }
    }
}
