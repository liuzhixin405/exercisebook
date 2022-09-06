using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace AspNetCore.Expressions
{
    public class ExpressionMapper
    {
        /// <summary>
        /// object存放的是Func
        /// </summary>
        private static Dictionary<string, object> _dictionary = new Dictionary<string, object>();

        public static TOut Trans<TIn,TOut>(TIn tin)
        {
            string key = $"funckey_{typeof(TOut).FullName}_{typeof(TIn).FullName}";

              //模拟执行操作   
              // Expression<Func<TIn, TOut>> func3 = (tin) => default(TOut);
              //类如   
             // Expression<Func<People, PeopleCopy>> func2 = (tin) => new PeopleCopy { Id=tin.Id,Name=tin.Name,Age=tin.Age};
            if (!_dictionary.ContainsKey(key))
            {
                ParameterExpression parameterExpression = Expression.Parameter(typeof(TIn), "p");   //(tin)
                List<MemberBinding> memberBindings = new List<MemberBinding>();

                TOut tout = Activator.CreateInstance<TOut>();
                var properties = tout.GetType().GetProperties();
                foreach (PropertyInfo item in properties)                    //属性赋值   Age=p.Age, Name=p.Name
                {
                    MemberExpression memberExpression = Expression.Property(parameterExpression, tin.GetType().GetProperty(item.Name));
                    MemberBinding memberBinding = Expression.Bind(item,memberExpression);
                    memberBindings.Add(memberBinding);
                }
                var fields = tout.GetType().GetFields();
                foreach (FieldInfo item in fields)             //字段赋值   Id = p.Id
                {
                    MemberExpression memberExpression = Expression.Field(parameterExpression, tin.GetType().GetField(item.Name));
                    MemberBinding memberBinding = Expression.Bind(item, memberExpression);
                    memberBindings.Add(memberBinding);
                }
                //   TIn 赋值给TOut
                MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(typeof(TOut)), memberBindings .ToArray());
                Expression<Func<TIn, TOut>> lambda = Expression.Lambda<Func<TIn, TOut>>(memberInitExpression, new ParameterExpression[] { parameterExpression});

                Func<TIn, TOut> func = lambda.Compile();
                _dictionary[key] = func;
            }

            return ((Func<TIn, TOut>)_dictionary[key]).Invoke(tin);
        }
    }
}
