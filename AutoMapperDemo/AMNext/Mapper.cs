using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AMNext
{
    internal class Mapper
    {
        public static Dictionary<(Type, Type), Delegate> _cache = new();
        public static T To<T>(object o)
        {
            var ta = o.GetType();
            var tb = typeof(T);
            var key = (ta, tb);
            if (!_cache.ContainsKey(key))
            {
                _cache[key] = CreateDelegate(ta, tb);
            }
            return (T) _cache[key].DynamicInvoke(o);  
        }
        private static Delegate CreateDelegate(Type inType,Type outType)
        {
            
            var param = Expression.Parameter(inType);
            var newExpression = Expression.New(outType.GetConstructor(Type.EmptyTypes));
            List<MemberBinding> bindings = new();
            foreach (var prop in inType.GetProperties())
            {
                var tbm = outType.GetProperty(prop.Name);
                if (tbm == null) continue;
                var pma = Expression.MakeMemberAccess(param, prop);
                var binding = Expression.Bind(tbm, pma);
                bindings.Add(binding);
            }
            var body = Expression.MemberInit(newExpression, bindings);

            return Expression.Lambda(body, false, param).Compile();
        }
    }
}
