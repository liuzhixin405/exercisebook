using VisitorNext;

IRepository repository = new Prostgres();
var operation = new GetInt();
repository.Visit(operation);
var res = operation.Result;
Console.WriteLine(res);