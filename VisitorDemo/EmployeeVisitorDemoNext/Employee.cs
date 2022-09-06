using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeVisitorDemoNext
{
    public abstract class Employee:IEmployee
    {
        private string name;
        private double income;
        private int vacationDays;
        public Employee(string name, double income, int vacationDays)
        {
            this.name = name;
            this.income = income;
            this.vacationDays = vacationDays;
        }

        public string Name { get => name; set => name = value; }
        public double Income { get => income; set => income = value; }
        public int VacationDays { get => vacationDays; set => vacationDays = value; }

        public virtual void Accept(IVisitor visitor)
        {
           
        }

        public override string ToString()
        {
            return $"{name} => 收入:{income},假期:{vacationDays}";
        }
    }
}
