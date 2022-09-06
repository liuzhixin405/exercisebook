using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AspNetCore.Expressions
{
    public class Reflection
    {
        public static TOut Trans<TIn, TOut>(TIn tin)
        {
            TOut tout = Activator.CreateInstance<TOut>();
            var properties = tout.GetType().GetProperties();
            foreach (PropertyInfo item in properties)
            {
                PropertyInfo pros = tin.GetType().GetProperty(item.Name);  //获取入参的属性

                item.SetValue(tout, pros.GetValue(tin));                 //赋值
            }
            var fields = tout.GetType().GetFields();
            foreach (FieldInfo item in fields)
            {
                FieldInfo field = tin.GetType().GetField(item.Name);  //获取入参的属性

                item.SetValue(tout, field.GetValue(tin));                 //赋值
            }

            return tout;
        }
    }
}
