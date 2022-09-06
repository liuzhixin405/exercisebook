using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryMethod.Classic
{
    public class Assembler
    {
        //marvellousWorks.practicalPattern.factoryMethod.customFactories
        private const string SectionName = "marvellousWorks.practicalPattern.factoryMethod.customFactories";

        private const string FactoryTypeName = "IFactory";

        private static Dictionary<Type, Type> dictionary = new Dictionary<Type, Type>();

        static Assembler()
        {
            NameValueCollection collection = (NameValueCollection)System.Configuration.ConfigurationManager.GetSection(SectionName);
            for (int i = 0; i < collection.Count; i++)
            {
                string target = collection.GetKey(i);
                string source = collection[i];
                dictionary.Add(Type.GetType(target), Type.GetType(source));
            }
        }

        /// <summary>
        /// 根据客户程序需要的抽象类型选择相应大的实体类型,并返回类型实例
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public object Create(Type type)
        {
            if (type == null || !dictionary.ContainsKey(type)) throw new NullReferenceException();
            Type targetType = dictionary[type];
            return Activator.CreateInstance(targetType);
        }
        /// <summary>
        /// 泛型方式调用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Create<T>()
        {
            return (T)Create(typeof(T));
        }
    }
}
