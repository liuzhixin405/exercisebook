using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace CustomerAOP.Frameworks
{
    public interface ICusServiceFactory<T>
    {
        T Imp { get; }
        ICusAOP _AOP { get; set; }
        Task<object?> Invoke(string methodName,object?[]? args);
    }
}
