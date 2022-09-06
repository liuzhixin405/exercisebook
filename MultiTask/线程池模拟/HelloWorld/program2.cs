using System;
using System.Collections.Generic;

namespace Test
{
    public delegate void MemoHandler(int x, int y, IDictionary<string, int> data);
    public class OverloadableDelegateInvoker
    {
        private MemoHandler handler;
        public OverloadableDelegateInvoker()
        {
            //Type type = typeof(MemoHandler);
            //Delegate d = Delegate.CreateDelegate(type, new C1(), "A");
            //d = Delegate.Combine(d, Delegate.CreateDelegate(type, new C2(), "S"));
            //d = Delegate.Combine(d, Delegate.CreateDelegate(type, new C3(), "M"));
            //handler = (MemoHandler)d;
            handler += new C1().A;
            handler += new C2().S;
            handler += new C3().M;
        }
        public void Memo(int x,int y,IDictionary<string,int> data)
        {
            handler(x, y, data);
        }
    }
    //class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        int result = 10;
    //        int expected = result;
    //        OverloadableDelegateInvoker invoker = new OverloadableDelegateInvoker();
    //        IDictionary<string, int> data = new Dictionary<String, int>();

    //        invoker.Memo(1, 2, data);
    //        Console.WriteLine(data["A"]);
    //        Console.Read();
    //    }
    //}

    public class C1
    {
        public void A(int x,IDictionary<string,int> data)
        {

        }
        public void A(int x, int y,IDictionary<string, int> data)
        {
            data.Add("A", x + y);
        }

    }
    public class C2
    {
        public void S(int x, IDictionary<string, int> data)
        {

        }
        public void S(int x, int y, IDictionary<string, int> data)
        {
            data.Add("S", x - y);
        }
    }

    public class C3
    {
      
        public void M(int x, int y, IDictionary<string, int> data)
        {
            data.Add("M", x * y);
        }
    }

    public interface IOrgnization { }

    public abstract class UserBase<TKey,TOrganization> where TOrganization : class, IOrgnization, new()
    {
        public abstract TOrganization Organization
        {
            get;
        }
        public abstract void Promotion(TOrganization newOrganization);
        delegate TOrganization OrganizationChangedHandler();

    }

    public interface IAttributeBuilder
    {
        IList<string> Log { get; }
        void BuildPartA();
        void BuildPartB();
        void BuildPartC();

    }

    [Director(1,"BuildPartC")]
    
    public class AttributeBuilder : IAttributeBuilder
    {
        private IList<string> log = new List<string>();
        public IList<string> Log => log;


        public void BuildPartA()
        {
            log.Add("a");
        }

        public void BuildPartB()
        {
            log.Add("b");
        }

        public void BuildPartC()
        {
            log.Add("c");
        }
    }
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class DirectorAttribute : Attribute, IComparable<DirectorAttribute>
    {
        private int priority;
        private string method;
        public DirectorAttribute(int priority,string method)
        {
            this.priority = priority;
            this.method = method;
        }
        public int Priority { get => this.priority; }
        public string Method { get => this.method; }
        public int CompareTo(DirectorAttribute other)
        {
            return other.priority - this.priority;
        }
    }
    public class Director
    {
        public void BuilUp(IAttributeBuilder builder)
        {
            object[] attributes = builder.GetType().GetCustomAttributes(typeof(DirectorAttribute), false);
            if (attributes.Length <= 0) return;
            DirectorAttribute[] directors = new DirectorAttribute[attributes.Length];
            for (int i = 0; i < attributes.Length; i++)
            {
                directors[i] = attributes[i] as DirectorAttribute;
                Array.Sort<DirectorAttribute>(directors);
                foreach (var item in directors)
                {
                    InvokeBuildPartMethod(builder, item);
                }
            }
        }

        private void InvokeBuildPartMethod(IAttributeBuilder builder, DirectorAttribute attribute)
        {
            switch (attribute.Method)
            {
                case "BuildPartA":
                    builder.BuildPartA();
                    break;
                case "BuildPartB":
                    builder.BuildPartB();
                    break;
                case "BuildPartC":
                    builder.BuildPartC();
                    break;
                default:
                    break;
            }
        }
    }

    class Program
    {
        static void Two_Main(string[] args)
        {
            IAttributeBuilder builder = new AttributeBuilder();
            Director director = new Director();
             director.BuilUp(builder);

            Console.WriteLine(builder.Log[0]);

           // Console.WriteLine(builder.Log[1]);

            Console.Read();
        }
    }

}
