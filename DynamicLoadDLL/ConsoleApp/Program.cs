using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.Loader;

namespace ConsoleApp
{
    internal class Program
    {
        
        static void Main(string[] args)
        {
            bool IsGo = true; //可以通过输入控制怎么循环
            while (IsGo)
            {
                ConcurrentDictionary<string, Type> _cd = new ConcurrentDictionary<string, Type>();
                var path = Directory.GetCurrentDirectory();

                string[] fileInfos = Directory.GetFiles(path).Where(f => f.Contains("Business.dll")).ToArray();  //拿到文件生成时间,判断是不是最新的。
                //文件数和_cd一样就不需要重新读文件
                var _AssemblyLoadContext = new AssemblyLoadContext(Guid.NewGuid().ToString("N"), true);
             
                foreach (var file in fileInfos)
                {
                    using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                    {
                        var assembly = _AssemblyLoadContext.LoadFromStream(fs);
                        //Type type = assembly.GetExportedTypes().Where(t => t.GetInterfaces().Contains(typeof(ITest))).FirstOrDefault();
                        Type[] types = assembly.GetExportedTypes().Where(t => t.IsClass).ToArray();
                        foreach (var type in types)
                        {
                            _cd.AddOrUpdate(type.FullName, type, (x, y) => y);
                        }
                    }
                }
                //无参无返回值
                foreach (var item in _cd)
                {
                    var type = item.Value;
                    object instance = Activator.CreateInstance(type);
                    MethodInfo[] methods = type.GetMethods().Where(x=>x.DeclaringType.FullName.Contains("Business")).ToArray();
                    foreach (var method in methods)
                    {
                        ParameterInfo[] parameters = method.GetParameters();
                       
                        List<object> pars_obj = new List<object>();
                        foreach (var parameter in parameters)
                        {
                            //参数类型判断
                            if (parameter.ParameterType.FullName.Equals("System.String")) //可以先把这些信息返回出去，再通过输入设备输入参数
                            {
                                pars_obj.Add("Jack");
                            }
                            if (parameter.ParameterType.FullName.Equals("System.Int32"))
                            {
                                pars_obj.Add(12);
                            }
                        }
                        method.Invoke(instance, pars_obj.ToArray());
                    }
                }
                Thread.Sleep(5000); //需要的类型放到ConsoleApp执行目录即可 
                
            }
        }
    }
}