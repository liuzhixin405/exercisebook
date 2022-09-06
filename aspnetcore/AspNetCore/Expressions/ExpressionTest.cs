using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AspNetCore.Expressions
{
    public abstract class Parent
    {
        public static Child Invoke(string parmeter)
        {
            return new Child();
        }
    }
    public class Child:Parent
    {

    }
    public class ExpressionTest
    {
        public void Show()
        {
            ConstantExpression left = ConstantExpression.Constant(123);
            ConstantExpression right = ConstantExpression.Constant(456);
            BinaryExpression expression = Expression<Func<int>>.Add(left, right);
            var result = Expression<Func<int>>.Lambda(expression, new ParameterExpression[] { })
                                              .Compile().DynamicInvoke();
            Console.WriteLine(result);
            //拼凑 m*n+m+n+2
            Expression<Func<int, int, int>> func = (m, n) => m * n + m + n + 3;
            var r= func.Compile().Invoke(1, 2);
            ParameterExpression parameterM = Expression.Parameter(typeof(int), "m");
            ParameterExpression parameterN = Expression.Variable(typeof(int), "n");
            BinaryExpression plus = DynamicExpression.Multiply(parameterM, parameterN);
            BinaryExpression binaryExpression = Expression<Func<int, int, int>>
                .Add(DynamicExpression.Add( 
                    DynamicExpression.Add( plus,parameterM), parameterN), ConstantExpression.Constant(2));
            var calculateResult = Expression<Func<int, int, int>>
                .Lambda(binaryExpression, new ParameterExpression[] { parameterM, parameterN })
                .Compile().DynamicInvoke(1, 2);

            //计算一下  100 * 5,   思路 100个五相加
            int m = 100;
            int n = 5;
            long sum = 0;
            while(n > 0)
            {
                sum += m;
                n--;
            }
            Console.WriteLine(sum);
        }
        
        public void ShowPeople()
        {
            Expression<Func<People, bool>> func = x => x.Id.ToString().Equals("1");
            ParameterExpression parameterExpression = Expression.Parameter(typeof(People), "x");
            FieldInfo field = typeof(People).GetField("Id");

            var fieldExp = Expression.Field(parameterExpression, field);
            ConstantExpression constantExpression = ConstantExpression.Constant("1");
            MethodInfo toString = typeof(int).GetMethod("ToString",new Type[0]);
            MethodInfo equals = typeof(string).GetMethod("Equals",new Type[] { typeof(string) });
            var toStringExp = Expression.Call(fieldExp, toString, new Expression[0]);
            var equalsExp = Expression.Call(toStringExp, equals, new Expression[] { constantExpression });

            Expression<Func<People, bool>> expression = Expression.Lambda<Func<People, bool>>(equalsExp, new ParameterExpression[] { parameterExpression });
            var boolResult = expression.Compile()(new People { Id = 2 });

        }

        public void ShowWhere()
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(People), "p");

            if (string.IsNullOrEmpty("name"))
            {

            }
            PropertyInfo name = typeof(People).GetProperty("Name");
            ConstantExpression Content = Expression.Constant("lzx",typeof(string));
            MemberExpression nameExp = Expression.Property(parameterExpression, name);
            MethodInfo contains = typeof(string).GetMethod("Contains",new Type[] {typeof(string) });
            var containsExp = Expression.Call(nameExp, contains, new Expression[] { Content });
            if (string.IsNullOrEmpty("Age"))
            {

            }

            var age = typeof(People).GetProperty("Age");
            var age5 = Expression.Constant(5);
            var ageExp = Expression.Property(parameterExpression, age);
            var greatorExp = Expression.GreaterThan(ageExp, age5);

            var body = Expression.AndAlso(containsExp, greatorExp);

            Expression<Func<People,bool>> expression = Expression.Lambda<Func<People,bool>>(body, new ParameterExpression[]{ parameterExpression });
            var whereResult = expression.Compile()(new People
            {
                Id = 1,
                Name = "lzx",
                Age = 18
            });
        }

        public void ClassCopy()
        {
            PeopleCopy pc = Reflection.Trans<People, PeopleCopy>(new People
            {
                Id = 1,
                Name = "test001",
                Age = 18
            });

            PeopleCopy pc2 = Serializable.DeSerializ<People, PeopleCopy>(new People
            {
                Id = 1,
                Name = "test001",
                Age = 18
            });
        }
    }

    public class People
    {
        public int Id;
        //public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class PeopleCopy
    {
        public int Id;
        //public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
