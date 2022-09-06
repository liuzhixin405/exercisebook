using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeVisitorDemoNext
{
    public class Manager : Employee
    {
        /// <summary>
        /// 分红
        /// </summary>
        private decimal divided;

        public Manager(string name, double income, int vacationDays,decimal divided) : base(name, income, vacationDays)
        {
            this.Divided = divided;
        }

        public decimal Divided { get => divided; set => divided = value; }

        public override void Accept(IVisitor visitor)
        {
            visitor.VisitManager(this);
        }
    }

    public class Charge : Employee
    {
        public Charge(string name, double income, int vacationDays) : base(name, income, vacationDays)
        {
        }

        public override void Accept(IVisitor visitor)
        {
            visitor.VisitCharge(this);
        }
    }
}
