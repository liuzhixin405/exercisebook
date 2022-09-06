using ObserverTwo;

X x = new X();

IUpdatebleObject a = new A();
IUpdatebleObject b = new B();
IUpdatebleObject c = new C();
Console.WriteLine("订阅前.................");
Console.WriteLine($"a.Data = {a.Data}");
Console.WriteLine($"b.Data = {b.Data}");
Console.WriteLine($"c.Data = {c.Data}");
x[0] = a;
x[1] = b;
x[2] = c;
x.Update(10);
Console.WriteLine("X发布data=10, 订阅后.................");
Console.WriteLine($"a.Data = {a.Data}");
Console.WriteLine($"b.Data = {b.Data}");
Console.WriteLine($"c.Data = {c.Data}");
