using System;
using System.Reflection;

namespace DotNetProxy
{
    class Program
    {
        static void Main(string[] args)
        {
            #region sample 1
            //var samepleProxy = (TagetInterface)SampleProxy.Create<TagetInterface, SampleProxy>();
            //samepleProxy.Write("here is invoke by proxy"); 
            #endregion
            var poxy = ProxyGenerator.Create<TagetInterface, SampleProxy>();
            poxy.Write("here is 2 invoke by proxy");
			var poxyImp = (TagetInterface) ProxyGenerator.Create(typeof(TagetInterface) , new SampleProxy());
            poxyImp.Write("here is 2 invoke by proxy");
            Console.Read();
        }
    }

    #region sample2 
    public interface TagetInterface
    {
        void Write(string writeSomething);
    }

    public class SampleProxy : IInterceptor
    {
        public object Intercept(MethodInfo targetMethod, object[] args)
        {
            Console.WriteLine(args[0]);
            return null;
        }
    }

    public interface IInterceptor
    {
        object Intercept(MethodInfo method, object[] paramters);
    }

    public class ProxyGenerator : DispatchProxy
    {
        private IInterceptor interceptor { get; set; }

        public static object Create(Type targetType, IInterceptor interceptor)
        {
             object proxy = GetProxy(targetType);
            MethodInfo method = typeof(ProxyGenerator).GetMethod(nameof(CreateInstance), BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, new[] { typeof(IInterceptor) }, null);
            method.Invoke(proxy, new object[] { interceptor });
            return proxy;
        }
        public static TTarget Create<TTarget, TInterceptor>(params object[] paremters) where TInterceptor : IInterceptor
        {
            object proxy = GetProxy(typeof(TTarget));
            MethodInfo method = typeof(ProxyGenerator).GetMethod(nameof(CreateInstance), BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, new[] { typeof(Type), typeof(object[]) }, null);
            method.Invoke(proxy, new object[] { typeof(TInterceptor), paremters });
            return (TTarget)proxy;
        }
        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            return this.interceptor.Intercept(targetMethod, args);

        }

        private static object GetProxy(Type targetType)
        {
            MethodInfo method = typeof(DispatchProxy).GetMethod(nameof(DispatchProxy.Create), new Type[] { });
            method = method.MakeGenericMethod(targetType, typeof(ProxyGenerator));
            return method.Invoke(null, null);
        }
        private void CreateInstance(Type interceptorType, object[] parameters)
        {
            this.interceptor = (IInterceptor)Activator.CreateInstance(interceptorType, parameters);
        }

        private void CreateInstance(IInterceptor interceptor)
        {
            this.interceptor = interceptor;
        }
    } 
    #endregion

    #region sample 1
    //public interface TagetInterface
    //{
    //    void Write(string writeSomething);
    //}

    //public class SampleProxy : DispatchProxy
    //{
    //    protected override object Invoke(MethodInfo targetMethod, object[] args)
    //    {
    //        Console.WriteLine(args[0]);
    //        return null;
    //    }
    //} 
    #endregion
}
