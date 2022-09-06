using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 服务注册类
{
    /// <summary>
    /// 代理工厂
    /// </summary>
    public interface IServiceProxyFactory
    {
        object CreateProxy(Type type);
        object CreateProxy(string key ,Type type);
        void RegisterProxyType(params Type[] types);
        T CreateProxy<T>() where T : class;
        T CreateProxy<T>(string key) where T : class;
    }
    /// <summary>
    /// 工厂扩展
    /// </summary>
    public static class ServiceProxyFactoryExtensions
    {
        public static T CreateProxy<T>(this IServiceProxyFactory serviceProxyFactory,Type proxyType)
        {
            return (T)serviceProxyFactory.CreateProxy(proxyType);
        }
    }
}
