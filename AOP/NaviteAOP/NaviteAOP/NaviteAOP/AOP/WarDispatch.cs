using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace NaviteAOP.AOP
{
    public class WarDispatch<T> : DispatchProxy where T : class
    {
        private T Target { get; set; }
        public static T Create<T>(T target) where T : class
        {
            var proxy = Create<T, WarDispatch<T>>() as WarDispatch<T>;
            proxy.Target = target;
            return proxy as T;
        }

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            Before().Wait();
            var result = targetMethod.Invoke(Target, args);
            After().Wait();
            return result;
        }

        Task Before()
        {
            return Task.CompletedTask;
        }

        Task After()
        {
            return Task.CompletedTask;
        }
    }
}
