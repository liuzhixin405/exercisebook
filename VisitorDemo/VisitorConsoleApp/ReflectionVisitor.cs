using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VisitorConsoleApp
{
    internal class ReflectionVisitor : IVisitor
    {

        public void Visit(IEmployee employee)
        {
            string typeName = employee.GetType().Name;
            string methodName = "Visit" + typeName;
            MethodInfo method = this.GetType().GetMethod(methodName)??throw new Exception($" {methodName} null ");
            method.Invoke(this, new object[] { employee });
        }

        public void VisitEmployee(IEmployee employee)
        {
            employee.Income *= 1.1;
            employee.VacationDays += 1;
        }

        public void VisitManager(IEmployee employee)
        {
            Manager manager = (Manager)employee;
            manager.Income *= 1.2;
            manager.VacationDays += 2;
        }
    }
}
