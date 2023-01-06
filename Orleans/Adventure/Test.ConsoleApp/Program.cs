using System.Reflection;
using Test.Interfaces;

namespace Test.ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
                //当程序集(Assembly)通过反射加载失败的时候会触发AssemblyResolve事件，这里注册AssemblyResolve事件的处理函数为CurrentDomain_AssemblyResolve
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
                //当类型(Type)通过反射加载失败的时候会触发TypeResolve事件，这里注册TypeResolve事件的处理函数为CurrentDomain_TypeResolve
                AppDomain.CurrentDomain.TypeResolve += CurrentDomain_TypeResolve;

                //这里通过调用Assembly.Load方法反射加载MessageDisplay程序集会失败，因为本项目中没有引用该程序集，而且MessageDisplay程序集的dll文件也不在本项目生成的bin目录下，也不在GAC中。所以这里会触发AssemblyResolve事件，调用处理函数CurrentDomain_AssemblyResolve来尝试执行自定义程序集加载逻辑，然后处理函数CurrentDomain_AssemblyResolve会为这里的Assembly.Load方法返回MessageDisplay.dll程序集
                var messageDisplayAssembly = Assembly.Load("Test.Implement");
                //使用反射动态调用MessageDisplayHelper类的构造函数
                var messageDisplayHelper = messageDisplayAssembly.CreateInstance("Test.Implement.FakeWorld");
                Console.WriteLine(messageDisplayHelper.ToString());

                //同样这里通过Type.GetType方法反射加载MessageDisplay程序集也会失败，会触发AssemblyResolve事件，调用处理函数CurrentDomain_AssemblyResolve来尝试执行自定义程序集加载逻辑，然后处理函数CurrentDomain_AssemblyResolve会为这里的Type.GetType方法返回所需要的程序集MessageDisplay.dll
                //和Assembly.Load方法不同，如果AssemblyResolve事件的处理函数CurrentDomain_AssemblyResolve为Type.GetType方法返回了null，Type.GetType方法并不会抛出异常，而是也返回一个null
                Type type = Type.GetType("Test.Implement.FakeWorld, Test.Implement");
                Console.WriteLine(type.ToString());

                //下面这里通过Type.GetType方法只反射类型MessageDisplay.MessageDisplayHelper，而不反射程序集MessageDisplay，所以会触发TypeResolve事件，调用处理函数CurrentDomain_TypeResolve来尝试执行自定义程序集加载逻辑，然后处理函数CurrentDomain_TypeResolve会为这里的Type.GetType方法返回所需要的程序集MessageDisplay.dll
                //同样如果TypeResolve事件的处理函数CurrentDomain_TypeResolve为Type.GetType方法返回了null，Type.GetType方法并不会抛出异常，而是也返回一个null
                type = Type.GetType("Test.Implement.FakeWorld, Test.Implement");
                Console.WriteLine(type.ToString());
                var obj =(IFakeWorld) Activator.CreateInstance(type);
                 Console.WriteLine($"结果：{obj.ReturnDestory().ConfigureAwait(false).GetAwaiter().GetResult()}");
                Console.WriteLine("Press any key to quit...");
                Console.ReadLine();
            }

            /// <summary>
            /// TypeResolve事件的处理函数，该函数用来自定义程序集加载逻辑
            /// </summary>
            /// <param name="sender">事件引发源</param>
            /// <param name="args">事件参数，从该参数中可以获取加载失败的类型的名称</param>
            /// <returns></returns>
            private static Assembly CurrentDomain_TypeResolve(object sender, ResolveEventArgs args)
            {
                //根据加载失败类型的名字找到其所属程序集并返回
                if (args.Name.Split(",")[0] == "Test.Implement.FakeWorld")
                {
                    //我们自定义的程序集加载逻辑知道MessageDisplay.MessageDisplayHelper类属于MessageDisplay程序集，而MessageDisplay程序集在C:\AssemblyResolverConsle\Reference\MessageDisplay.dll这个路径下，所以这里加载这个路径下的dll文件作为TypeResolve事件处理函数的返回值
                    return Assembly.LoadFile(@"G:\Users\GitHub\exercisebook\Orleans\Adventure\Test.Implement\bin\Debug\net7.0\Test.Implement.dll");
                }

                //如果TypeResolve事件的处理函数返回null，说明TypeResolve事件的处理函数也不知道加载失败的类型属于哪个程序集
                return null;
            }

            /// <summary>
            /// AssemblyResolve事件的处理函数，该函数用来自定义程序集加载逻辑
            /// </summary>
            /// <param name="sender">事件引发源</param>
            /// <param name="args">事件参数，从该参数中可以获取加载失败的程序集的名称</param>
            /// <returns></returns>
            private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
            {
                //根据加载失败程序集的名字找到该程序集并返回
                if (args.Name.Split(",")[0] == "Test.Implement")
                {
                    //我们自定义的程序集加载逻辑知道MessageDisplay程序集在C:\AssemblyResolverConsle\Reference\MessageDisplay.dll这个路径下，所以这里加载这个路径下的dll文件作为AssemblyResolve事件处理函数的返回值
                    return Assembly.LoadFile(@"G:\Users\GitHub\exercisebook\Orleans\Adventure\Test.Implement\bin\Debug\net7.0\Test.Implement.dll");
                }

                //如果AssemblyResolve事件的处理函数返回null，说明AssemblyResolve事件的处理函数也无法找到加载失败的程序集，那么整个程序就会抛出异常报错
                return null;
            }
        }
}