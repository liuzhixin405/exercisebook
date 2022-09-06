using AMNext;
using System.Linq.Expressions;

//Expression<Func<TestA, TestB>> exp = a => new TestB { Id = a.Id, Name = a.Name };
var tb=Mapper.To<TestB>(new TestA { Id = 3, Name = "TestAAA" });

Console.WriteLine(tb.ToJson());