using StrategyPattern03.Provider;
using System.Text;

namespace StrategyPattern03
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }

        
    }
    internal static class Extensions
    {
        internal static ObjectPool<StringBuilder> CreateStringBuilderPool(this ObjectPoolProvider provider)
        {
            return provider.Create<StringBuilder>(new StringBuilderPooledObjectPolicy());
        }
        internal static ObjectPool<StringBuilder> CreateStringBuilderPool(this ObjectPoolProvider provider,int initialCapacity,int maximmRetainedCapacity)
        {
            var policy = new StringBuilderPooledObjectPolicy
            {
                InitialCapacity = initialCapacity,
                MaximmRetainedCapacity = maximmRetainedCapacity
            };
            return provider.Create<StringBuilder>(policy);
        }
    }
}