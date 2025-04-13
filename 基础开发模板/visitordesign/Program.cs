namespace visitordesign
{
    internal class Program
    {
        static void Main(string[] args)
        {
           EmployeeCollection employees = new EmployeeCollection();
            employees.Add(new Employee("Jack", 10000, 14));
            employees.Add(new Employee("Jill", 12000, 10));
            employees.Add(new Manager("John", 15000, 20, "HR"));
            employees.Add(new Manager("Jane", 18000, 15, "IT"));

            Console.WriteLine($"普通员工：姓名：{employees[0].Name}, 薪水{employees[0].Income},假期：{employees[0].VacationDays}天");
            Console.WriteLine($"管理者：姓名：{employees[3].Name}, 薪水{employees[3].Income},假期：{employees[3].VacationDays}天");
            employees.Accept(new ExtraVacationVisitor());
            employees.Accept(new RaiseSalaryVisitor());
            Console.WriteLine("调薪后");
            Console.WriteLine($"普通员工：姓名：{employees[0].Name}, 薪水{employees[0].Income},假期：{employees[0].VacationDays}天");
            Console.WriteLine($"管理者：姓名：{employees[3].Name}, 薪水{employees[3].Income},假期：{employees[3].VacationDays}天");
        }
    }

    class ExtraVacationVisitor : IVisitor
    {
     
        public void VisitEmployee(IEmployee employee)
        {
            employee.VacationDays += 1;
        }

        public void VisitManager(Manager manager)
        {
            manager.VacationDays += 2;
        }
    }

    class RaiseSalaryVisitor : IVisitor
    {
        public void VisitEmployee(IEmployee employee)
        {
            employee.Income *= 1.1;
        }

        public void VisitManager(Manager manager)
        {
            manager.Income *= 1.2;
        }
    }
}
