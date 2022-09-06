using IOC.Core;
using NUnit.Framework;
using System;
using System.Diagnostics;

namespace IOC.CoreTest
{
    public interface ITack
    {
        string Invoke();
    }
    public class Tack : ITack
    {
        public Tack()
        {

        }
        public string Val { get; set; }
        public Tack(string val)
        {
            Val = val;
        }
        public string Invoke()
        {
            if (!string.IsNullOrEmpty(Val))
            {
                return $"{Val} Go";
            }
            return "Go";
        }
    }
    
    public class NoCtor
    {
        public string Invoke()
        {
            return "Go";
        }
    }

    #region adapte
    public interface IAdapter
    {
        void specialRequest();
    }
    public interface IRequire
    {
        void Request();
    }
    public abstract class AbstractAdaptee<T> : IAdapter where T : IRequire, new()
    {
        public virtual void specialRequest()
        {
            new T().Request();
        }
    }
    public class Implement : IRequire
    {
        public void Request()
        {
            Trace.WriteLine("i am normal request");
        }
    }

    public class Adaptee : AbstractAdaptee<Implement>
    {
        public override void specialRequest()
        {
            Trace.WriteLine("special request");
            base.specialRequest();
        }
    } 
    #endregion

    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var tack = new Cat().Register<ITack, Tack>(Lifetime.Root);
            Assert.IsTrue(tack.GetService<ITack>().Invoke() == "Go");
        }
        [Test]
        public void Test2()
        {
            var tack = new Cat().Register(new Tack("Hello"));
            Assert.IsTrue(tack.GetService<Tack>().Invoke() == "Hello Go");
        }
        [Test]
        public void Test3()
        {
            var ctor = new Cat().Register(new NoCtor()).GetService<NoCtor>(); //使用前必须先注册
            Assert.IsTrue(ctor.Invoke() == "Go");
        }
        [Test]
        public void Test4()
        {
            var special = new Cat().Register<IAdapter, Adaptee>(Lifetime.Transient).GetService<IAdapter>();
            special.specialRequest();
            Assert.Pass();
        }

    }
}