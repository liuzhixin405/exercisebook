using EmployeeVisitorDemoNext;

IEmployeeCollection collection = new IEmployeeCollection();
var manager = new Manager("经理程", 25000, 14,10000);
collection.Add(manager);

var charge = new Charge("主管张", 10000, 10);
collection.Add(charge);
Console.WriteLine($"manager年前: {manager} ,分红:{manager.Divided}");
Console.WriteLine($"charge年前: {charge}");
collection.Accept(new RaiseSalaryVisitor());
collection.Accept(new ExtraVacationVisitor());

Console.WriteLine($"manager年后: {manager},,分红:{manager.Divided}");
Console.WriteLine($"charge年后: {charge}");