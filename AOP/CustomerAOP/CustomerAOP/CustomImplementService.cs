using System.ComponentModel.DataAnnotations;
using System.Reflection;
using CustomerAOP.Frameworks;

namespace CustomerAOP
{
    public class DefaultAOP : ICusAOP
    {
        public virtual Task After()
        {
            Console.WriteLine($"方法调用之后{DateTime.UtcNow}");
            return Task.CompletedTask;
        }

        public virtual async Task Before()
        {
            Console.WriteLine($"方法调用之前{DateTime.UtcNow}");
            await Task.Delay(2000);
        }
    }

    public class DefaultServiceFactory<T> : ICusServiceFactory<T>
    {
        private T _t { get; set; }
        public DefaultServiceFactory(T t, ICusAOP aOP)
        {
            _t = t;
            _AOP = aOP;
        }

        public ICusAOP _AOP { get; set; }
        public T Imp => _t;

        private async Task<object?> Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            await _AOP.Before();
            var result = targetMethod.Invoke(_t, args);
            await _AOP.After();
            return result;
        }

        public async Task<object?> Invoke(string methodName,object?[]? args)
        {
            if(string.IsNullOrEmpty(methodName))
            {
                throw new Exception("需要调用的方法名不能为空");
            }
            var method = _t.GetType().GetMethods().Where(x=>x.DeclaringType.Name== _t.GetType().Name).Where(x=>x.Name.ToLower().Equals(methodName?.ToLower())).FirstOrDefault();
            if (method == null)
            {
                throw new Exception($"方法{methodName}不存在请检查");
            }
            var task= Invoke(method, args);
            return await task;
        }
    }
}