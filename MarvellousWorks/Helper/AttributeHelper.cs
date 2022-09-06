using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public static class AttributeHelper
    {
        /// <summary>
        /// 获取某个类型指定类型的集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IList<T> GetCustomAttributes<T>(Type type) where T : Attribute
        {
            if (type == null) throw new ArgumentNullException("type");
            if (!type.IsDefined(typeof(T), false))
            {
                throw new Exception("无此特性");
            }
            T[] attributes = type.GetCustomAttributes(typeof(T), false) as T[];
            return attributes.Length == 0 ? null : attributes.ToList();
        }

        /// <summary>
        /// 获取某个类型包含指定属性的方法集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IList<MethodInfo> GetMethodsWithCustomAttribute<T>(Type type) where T : Attribute
        {
            if (type == null) throw new ArgumentNullException("type");
            MethodInfo[] methods = type.GetMethods();
            if (methods == null || methods.Length == 0) return null;
            IList<MethodInfo> result = new List<MethodInfo>();
            foreach (MethodInfo method in methods)
            {
                if (method.IsDefined(typeof(T), false))
                {
                    result.Add(method);
                }
            }

            return result.Count == 0?null: result;
        }

        /// <summary>
        /// 获取某个方法指定类型属性的集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <returns></returns>
        public static IList<T> GetMethodCustomAttributes<T>(MethodInfo method) where T : Attribute
        {
            if (method == null) throw new ArgumentNullException(nameof(MethodInfo));
            if (method.IsDefined(typeof(T), false))
            {
                T[] attributes = method.GetCustomAttributes(typeof(T), false) as T[];
                return attributes == null ? null : attributes.ToList();
            }
            throw new Exception("此方法没有该属性");
        }
        /// <summary>
        /// 获取某个方法的指定属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="method"></param>
        /// <returns></returns>
        public static T GetMethodCustomAttribute<T>(MethodInfo method) where T : Attribute
        {
            if (method == null) throw new ArgumentNullException(nameof(MethodInfo));
            T attribute = method.GetCustomAttribute(typeof(T), false) as T;
            return attribute?? null;
        }
    }
}
