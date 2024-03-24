namespace 代理模式
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IRequest<String> request = new RealRequest();
            request.Request("proxy");
            
            ITarget adpter = new Adaptee<AdapteeImpl>();
            adpter.Request();
        }
    }

    internal interface IRequest<T>
    {
        void Request(T request);
    }
   

    internal class RealRequest : IRequest<string>
    {
        public void Request(string request)
        {
            Console.WriteLine($"Request: {request}");
        }
    }

    internal class ProxyRequest : IRequest<string>
    {
        private readonly IRequest<string> _request;
        public ProxyRequest(IRequest<string> request)
        {
            _request = request;
        }
        public void Request(string request)
        {
            _request.Request(request);
        }
    }

    /// <summary>
    /// 适配器
    /// </summary>
    internal interface ITarget
    {
        void Request();
    }
    public class TargetImpl : ITarget
    {
        public void Request()
        {
            Console.WriteLine("this is a real request");
        }
    }
    internal interface IAdaptee
    {
        void SpecifiedRequest();
    }
    public class AdapteeImpl : IAdaptee
    {
        public void SpecifiedRequest()
        {
            Console.WriteLine("this is a adapter for request");
        }
    }
    internal abstract class GenericAdapterBase<T>:ITarget where T: IAdaptee,new()
    {
        public virtual void Request()
        {
            new T().SpecifiedRequest();
        }
    }

    internal class Adaptee<T> : GenericAdapterBase<T> where T: IAdaptee,new()
    {
        public override void Request()
        {
            base.Request();
        }
    }
}
