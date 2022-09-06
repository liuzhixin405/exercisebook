using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DataProtectConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Foo
            //var serviceCollection = new ServiceCollection();
            //serviceCollection.AddDataProtection();
            //var services = serviceCollection.BuildServiceProvider();
            //var instance = ActivatorUtilities.CreateInstance<MyClass>(services);
            //instance.RunSample();

            #endregion

            #region Baz

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddDataProtection();
            var services = serviceCollection.BuildServiceProvider();
            var instance = ActivatorUtilities.CreateInstance<ProtectProject>(services);
            var proData = instance.RunSample("你好啊");
            var newInstance = ActivatorUtilities.CreateInstance<UnProtectProject>(services);
            newInstance.RunSample(proData);

            #endregion

            Console.Read();

        }
    }

    public class MyClass
    {
        IDataProtector _protector;
        public MyClass(IDataProtectionProvider dataProtector)
        {
            _protector = dataProtector.CreateProtector("Contoso.MyClass.v1");
        }

        public void RunSample()
        {
            Console.Write("Enter input:");
            string input = Console.ReadLine();
            string protectedPayload = _protector.Protect(input);
            Console.WriteLine($"protect returned: {protectedPayload}");

            string unprotectedPayload = _protector.Unprotect(protectedPayload);

            Console.WriteLine($"unprotect returned: {unprotectedPayload}");
        }
    }

    public class ProtectProject
    {
        IDataProtector _protector;
        public ProtectProject(IDataProtectionProvider dataProtector)
        {
            _protector = dataProtector.CreateProtector("Contoso.MyClass.v1"); //重点两边保证一致即可 动态赋值到配置文件， 随时可以更改 只要两边保持好就可以了
        }
        public string RunSample(string str)
        {
            string protectedPayload = _protector.Protect(str);
            Console.WriteLine($"protect returned: {protectedPayload}");
            return protectedPayload;
        }
    }

    public class UnProtectProject
    {
        IDataProtector _protector;
        public UnProtectProject(IDataProtectionProvider dataProtector)
        {
            _protector = dataProtector.CreateProtector("Contoso.MyClass.v1");
        }
        public void RunSample(string str)
        {
            var result = _protector.Unprotect(str);

            Console.WriteLine($"unprotect returned: {result}");
        }
    }
}
