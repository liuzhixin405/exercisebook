using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace AspNetCore.Expressions
{
    public class ExpressionGenericMapper<TIn,TOut> where TIn :class 
                                                   where TOut:class
    {
        private static Func<TIn, TOut> _FUNC = null;
        static ExpressionGenericMapper()
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(TIn), "p");   //(tin)
            List<MemberBinding> memberBindings = new List<MemberBinding>();

            TOut tout = Activator.CreateInstance<TOut>();
            var properties = tout.GetType().GetProperties();
            foreach (PropertyInfo item in properties)                    //属性赋值   Age=p.Age, Name=p.Name
            {
                MemberExpression memberExpression = Expression.Property(parameterExpression, typeof(TIn).GetProperty(item.Name));
                MemberBinding memberBinding = Expression.Bind(item, memberExpression);
                memberBindings.Add(memberBinding);
            }
            var fields = tout.GetType().GetFields();
            foreach (FieldInfo item in fields)             //字段赋值   Id = p.Id
            {
                MemberExpression memberExpression = Expression.Field(parameterExpression, typeof(TIn).GetField(item.Name));
                MemberBinding memberBinding = Expression.Bind(item, memberExpression);
                memberBindings.Add(memberBinding);
            }
            //   TIn 赋值给TOut
            MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(typeof(TOut)), memberBindings.ToArray());
            Expression<Func<TIn, TOut>> lambda = Expression.Lambda<Func<TIn, TOut>>(memberInitExpression, new ParameterExpression[] { parameterExpression });

            _FUNC = lambda.Compile();
            
        }

        public static TOut Trans(TIn tin)
        {
            return _FUNC(tin);
        }
    }
}
