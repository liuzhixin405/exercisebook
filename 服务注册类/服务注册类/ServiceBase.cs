using System;

namespace 服务注册类
{
    public abstract class ServiceBase
    {
        public T GetService<T>()
        {
            return ServiceLocator.GetService<T>();
        }

        public T GetService<T>(String key)
        {
            return ServiceLocator.GetService<T>(key);
        }

        public object GetService(Type type)
        {
            return ServiceLocator.GetService(type);
        }
        public object GetService(string key, Type type)
        {
            return ServiceLocator.GetService(key, type);
        }
    }
}
