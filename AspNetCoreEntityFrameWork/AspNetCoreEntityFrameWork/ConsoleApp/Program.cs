using Business;
using IBusiness;
using System;
using System.Reflection;
using Models;
using Castle.DynamicProxy;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using EF.Core;
using CacheManager.Core;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            User user = new User { Name = "Zoe", Age = 18 };

            #region 1
            //动态代理
            //1. 创建代理对象
            IUserService userService1 = DispatchProxy.Create<IUserService, MyDecorator>();
            //2. 因为调用的是实例方法，需要传提具体类型
            ((MyDecorator)userService1).TargetClass = new UserService();
            userService1.AddUser(user);

            #endregion
            // 2
            ProxyGenerator generator = new ProxyGenerator();
            //var u = generator.CreateInterfaceProxyWithTarget<IUserService>(new UserService(), new MyIntercept());

            //3
            var services = new ServiceCollection();

            var scope = services.BuildServiceProvider(validateScopes: true);
            
            CastleInterceptor castleInterceptor = new CastleInterceptor(scope);

            var u = (IUserService)generator.CreateInterfaceProxyWithTarget(typeof(IUserService), new UserService(), castleInterceptor);
            u.AddUser(user);
            u.AddUser(user);  //缓存
            u.Print();
            Console.ReadLine();
        }
    }

    #region 1
    public class MyDecorator : DispatchProxy
    {
        //具体类型
        public object TargetClass { get; set; }
        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            Console.WriteLine("增加用户前执行业务");

            //调用原有方法
           

            Console.WriteLine("增加用户后执行业务");

            return targetMethod.Invoke(TargetClass, args); ;
        }
    }
    #endregion

    #region 2
    public class MyIntercept : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            //执行原有方法之前
            Console.WriteLine("增加用户前执行业务");

            //执行原有方法
            invocation.Proceed();

            //执行原有方法之后
            Console.WriteLine("增加用户后执行业务");
        }
    }
    #endregion
}
