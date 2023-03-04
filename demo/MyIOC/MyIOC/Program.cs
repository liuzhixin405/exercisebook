using System.ComponentModel.Design;
using System.Reflection;

namespace MyIOC
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var fac = new DefaultFactory();
            Test test = (Test)fac.GetObject("MyIOC.Test");
            Console.WriteLine(test.GetResult(1, 2));
        }
    }

    [IOCService]
    public class Test
    {
        public Test()
        {

        }
        public Cal Cal { get; set; }
        public int GetResult(int x ,int y)
        {
           return Cal.Add(x, y);
        }
    }
    [IOCService]
    public class Cal
    {
        public Cal()
        {

        }
        public int Add(int x,int y)
        {
            return x + y;
        }
    }
    public class IOCService : Attribute { }
    public class DefaultFactory
    {
        private Dictionary<string, Type> ioctypeContainter = new();
        private Dictionary<String, object> iocContainer = new();
        public DefaultFactory()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] types = assembly.GetTypes();
            foreach (var type in types)
            {
                ioctypeContainter.Add(type.FullName, type);
            }
            foreach (var type in types)
            {
                IOCService svc = type.GetCustomAttribute<IOCService>();
                if (svc != null)
                {
                    var _object = CreateObject(type, types);
                    iocContainer.Add(type.FullName, _object); 
                }
            }
        }
        public object GetObject(string name)
        {
            var res= iocContainer[name];
            
            return res;
        }
        public object CreateObject(Type type, Type[] types)
        {
            object _object = Activator.CreateInstance(type) ?? throw new ArgumentNullException($"{nameof(type)}");
            PropertyInfo[] propertyInfos = type.GetProperties();
            foreach (var propertyInfo in propertyInfos)
            {
                //    foreach (var sub in types)
                //    {
                //        if (propertyInfo.PropertyType.Equals(sub))
                //        {
                //            var objVal = CreateObject(type, types);
                //            propertyInfo.SetValue(_object, objVal);
                //        }
                //    }
                Type sub = ioctypeContainter[propertyInfo.PropertyType.FullName];
                var objVal = CreateObject(sub, types);
                propertyInfo.SetValue(_object, objVal);
            }
            return _object;
        }
    }
}