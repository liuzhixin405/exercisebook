using IOC.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IOC.CoreTest
{
    public class CatTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test()
        {
            var root = new Cat().Register<IFoo, Foo>(Lifetime.Transient)
                  .Register<IBar>(null, _ => new Bar(), Lifetime.Self)
                  .Register<IBaz, Baz>(Lifetime.Root)
                  .Register(typeof(IFoobar<,>),typeof(Foobar<,>),Lifetime.Transient)
                  .Register(Assembly.GetEntryAssembly());
            var cat1 = root.CreateChild();
            var cat2 = root.CreateChild();
            void GetServices<TService>(Cat cat)
            {
                cat.GetService<TService>();
                cat.GetService<TService>();
            }
            GetServices<IFoo>(cat1);
            GetServices<IBar>(cat1);
            GetServices<IBaz>(cat1);
            GetServices<IQux>(cat1);
            Trace.WriteLine("***************************");
            GetServices<IFoo>(cat2);
            GetServices<IBar>(cat2);
            GetServices<IBaz>(cat2);
            GetServices<IQux>(cat2);
            var foobar = (Foobar<IFoo,IBar>)cat1.GetService<IFoobar<IFoo,IBar>>();
            Assert.IsTrue(foobar.Foo is Foo);
            Assert.IsTrue(foobar.Bar is Bar);
            Assert.IsFalse(false);

        }

        [Test]
        public void TestSecond()
        {
            var services = new Cat().Register<Base, Foo>(Lifetime.Transient)
                 .Register<Base, Bar>(Lifetime.Transient)
                 .Register<Base, Baz>(Lifetime.Transient).GetServices<Base>();
            Assert.IsTrue(services.OfType<Foo>().Any());
            Assert.IsTrue(services.OfType<Bar>().Any());
            Assert.IsTrue(services.OfType<Baz>().Any());
        }
    }

    public interface IFoo { }
    public interface IBar { }
    public interface IBaz { }
    public interface IQux { }
    public interface IFoobar<T1, T2> { }

    public class Base : IDisposable
    {
        public Base()
        {
            Trace.WriteLine($"Instance of {GetType().Name } is created .");
        }
        public void Dispose()
        {
            Trace.WriteLine($"Instance of {GetType().Name} is disposed .");
        }
    }

    public class Foo : Base, IFoo { }
    public class Bar : Base, IBar { }
    public class Baz : Base, IBaz { }
    [MapTo(typeof(IQux),Lifetime.Root)]
    public class Qux : Base, IQux { }
    public class Foobar<T1, T2> : IFoobar<T1, T2>
    {
        public IFoo Foo { get; }
        public IBar Bar { get; }
        public Foobar(IFoo foo,IBar bar)
        {
            Foo = foo;
            Bar = bar;
        }
    }
}

