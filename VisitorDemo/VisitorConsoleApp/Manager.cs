using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorConsoleApp
{

    internal class Employee : IEmployee
    {
        private string name;
        private double income;
        private int vacationDays;

        public Employee(string name,double income,int vacationDays)
        {
            this.name = name;
            this.income = income;
            this.vacationDays = vacationDays;
        }
        public string Name { get => name; set => name = value; }
        public double Income { get => income; set => income = value; }
        public int VacationDays { get => vacationDays; set => vacationDays = value; }

        public void Accept(IVisitor visitor)
        {
           visitor.Visit(this);
        }
    }
    internal class Manager : Employee
    {
        private string department;

        public Manager(string name, double income, int vacationDays,string department) : base(name, income, vacationDays)
        {
            this.Department = department;
        }

        public string Department { get => department; set => department = value; }

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
