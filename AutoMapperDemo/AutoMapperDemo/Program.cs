using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AutoMapperDemo
{
    public  class Program 
    {
        public static void Main(string[] args)
        {
            Expression<Func<TestA, TestB>> exp = a => new TestB { Id = a.Id, Name = a.Name };
            var ta = typeof(TestA);
            var tb = typeof(TestB);
            var param = Expression.Parameter(ta);
            var newExpression = Expression.New(tb.GetConstructor(Type.EmptyTypes));
            List<MemberBinding> bindings = new();
            foreach (var prop in ta.GetProperties())
            {
                var tbm = tb.GetProperty(prop.Name);
                if (tbm == null) continue;
                var pma = Expression.MakeMemberAccess(param, prop);
                var binding = Expression.Bind(tbm, pma);
                bindings.Add(binding);
            }
            var body = Expression.MemberInit(newExpression, bindings);
          
           var obj = Expression.Lambda(body, false, param).Compile().DynamicInvoke(new TestA { Id=1, Name="TestA"});

        }
    }
}
