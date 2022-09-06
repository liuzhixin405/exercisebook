using VisitorConsoleApp;

EmployeeCollection employees = new EmployeeCollection();
employees.Add(new Employee("joe", 25000, 14));
employees.Add(new Manager("alice", 22000, 14, "sales"));

employees.Add(new Employee("peter", 15000, 7));

employees.Accept(new ReflectionVisitor());

Console.WriteLine($"joe => income:{employees[0].Income},day:{employees[0].VacationDays}");
Console.WriteLine($"alice => income:{employees[1].Income},day:{employees[1].VacationDays} ,deparetment:{((Manager)employees[1]).Department}");
Console.WriteLine($"peter => income:{employees[2].Income},day:{employees[2].VacationDays}");