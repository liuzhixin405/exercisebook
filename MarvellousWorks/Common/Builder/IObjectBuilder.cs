using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Builder
{
    public interface IObjectBuilder
    {
        /// <summary>
        /// 创建类型实例 有参构造
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <returns></returns>
        T BuildUp<T>(object[] args);
        /// <summary>
        /// 创建类型实例 无参
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T BuildUp<T>() where T:new();

        /// <summary>
        /// 按照目标返回的类型，加工指定类型名称对应的类型实例。
        /// 目标类型可以为接口、抽象类等抽象类型，typeName一般为实体类名称。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="typeName"></param>
        /// <returns></returns>
        T BuildUp<T>(string typeName);
        /// <summary>
        /// 按照目标类型，通过调用指定名称类型的构造函数，生成目标类型实例。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="typeName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        T BuildUp<T>(string typeName,object[] args);
    }
}
