using ObserverOne;

A a = new A();
B b = new B();
C c = new C();

Console.WriteLine("订阅前.................");
Console.WriteLine($"a.Data = {a.Data}");
Console.WriteLine($"b.Count = {b.Count}");
Console.WriteLine($"c.N = {c.N}");

X x =new X();
x.instanceA = a;
x.instanceB = b;
x.instanceC = c;
x.SetData(10); 
Console.WriteLine("X发布data=10, 订阅后.................");
Console.WriteLine($"a.Data = {a.Data}");
Console.WriteLine($"b.Count = {b.Count}");
Console.WriteLine($"c.N = {c.N}");
