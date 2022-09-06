using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 服务注册类
{
    /// <summary>
    /// ProxyServiceBase默认的服务代理工厂 可以涵盖抽象远程调用 抽象类型转换和抽象服务提供者IServiceProvider,暂未实现,IBaseService 暂未使用 
    /// </summary>
    public class BaseService:ProxyServiceBase,IBaseService
    {
        /// <summary>
        /// 有返回值
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="callback"></param>
        /// <returns></returns>
        protected TOut ExcuteFunc<TIn,TOut>(Func<TIn,TOut> callback)
        {
            var container = ServiceLocator.Current;
            using (var scope = container.BeginLifetimeScope())
            {
                var service = scope.Resolve<TIn>();
                return callback(service);
            }
        }

        /// <summary>
        /// 执行有返回值的数据
        /// </summary>
        /// <returns></returns>
        protected TOut ExcuteFunc<TIn1, TIn2, TOut>(Func<TIn1, TIn2, TOut> callback)
        {
            var contianer = ServiceLocator.Current;
            using (var scope = contianer.BeginLifetimeScope())
            {
                var service1 = scope.Resolve<TIn1>();
                var service2 = scope.Resolve<TIn2>();
                return callback(service1, service2);
            }
        }
        /// <summary>
        /// 执行有返回值的数据
        /// </summary>
        /// <returns></returns>
        protected TOut ExcuteFunc<TIn1, TIn2, TIn3, TOut>(Func<TIn1, TIn2, TIn3, TOut> callback)
        {
            var contianer = ServiceLocator.Current;
            using (var scope = contianer.BeginLifetimeScope())
            {
                var service1 = scope.Resolve<TIn1>();
                var service2 = scope.Resolve<TIn2>();
                var service3 = scope.Resolve<TIn3>();
                return callback(service1, service2, service3);
            }
        }
        /// <summary>
        /// 执行有返回值的数据
        /// </summary>
        /// <returns></returns>
        protected TOut ExcuteFunc<TIn1, TIn2, TIn3, TIn4, TOut>(Func<TIn1, TIn2, TIn3, TIn4, TOut> callback)
        {
            var contianer = ServiceLocator.Current;
            using (var scope = contianer.BeginLifetimeScope())
            {
                var service1 = scope.Resolve<TIn1>();
                var service2 = scope.Resolve<TIn2>();
                var service3 = scope.Resolve<TIn3>();
                var service4 = scope.Resolve<TIn4>();
                return callback(service1, service2, service3, service4);
            }
        }
        /// <summary>
        /// 执行有返回值的数据
        /// </summary>
        /// <returns></returns>
        protected TOut ExcuteFunc<TIn1, TIn2, TIn3, TIn4, TIn5, TOut>(Func<TIn1, TIn2, TIn3, TIn4, TIn5, TOut> callback)
        {
            var contianer = ServiceLocator.Current;
            using (var scope = contianer.BeginLifetimeScope())
            {
                var service1 = scope.Resolve<TIn1>();
                var service2 = scope.Resolve<TIn2>();
                var service3 = scope.Resolve<TIn3>();
                var service4 = scope.Resolve<TIn4>();
                var service5 = scope.Resolve<TIn5>();
                return callback(service1, service2, service3, service4, service5);
            }
        }
        /// <summary>
        /// 执行没有返回值的数据
        /// </summary>
        /// <typeparam name="T">服务类型</typeparam>
        /// <param name="callback"></param>
        protected void ExcuteAction<T>(Action<T> callback)
        {
            var contianer = ServiceLocator.Current;
            using (var scope = contianer.BeginLifetimeScope())
            {
                var service = scope.Resolve<T>();
                callback(service);
            }
        }
        /// <summary>
        /// 执行没有返回值的数据
        /// </summary>
        /// <param name="callback"></param>
        protected void ExcuteAction<T1, T2>(Action<T1, T2> callback)
        {
            var contianer = ServiceLocator.Current;
            using (var scope = contianer.BeginLifetimeScope())
            {
                var service1 = scope.Resolve<T1>();
                var service2 = scope.Resolve<T2>();
                callback(service1, service2);
            }
        }
        /// <summary>
        /// 执行没有返回值的数据
        /// </summary>       
        /// <param name="callback"></param>
        protected void ExcuteAction<T1, T2, T3>(Action<T1, T2, T3> callback)
        {
            var contianer = ServiceLocator.Current;
            using (var scope = contianer.BeginLifetimeScope())
            {
                var service1 = scope.Resolve<T1>();
                var service2 = scope.Resolve<T2>();
                var service3 = scope.Resolve<T3>();
                callback(service1, service2, service3);
            }
        }
    }
}
