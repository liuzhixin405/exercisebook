using System;

namespace 责任链简单
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Handler handler = new ConcreteHandlerFoo(); 
            handler = new ConcreteHandlerBar(handler);
            handler = new ConcreteHandlerBaz(handler);
            handler = new ConcreteHandlerQux(handler);
            handler.HandleRequest(33);
        }
    }

    internal abstract class Handler
    {
        /// <summary>
        /// 这里可以通过方法或者属性都可以实现，不一定要构造函数,但是构造比较方便简洁实用
        /// </summary>
        protected Handler successor;
        internal Handler(Handler handler) => successor = handler;

        internal abstract void HandleRequest(int request);
    }


    internal class ConcreteHandlerFoo : Handler
    {
        public ConcreteHandlerFoo(Handler handler=null) : base(handler) //handler=null 方便初始化，后续不能空
        {
        }

        internal override void HandleRequest(int request)
        {
            if (request >= 0 && request < 10)
            {
                Console.WriteLine($"{this.GetType().Name} handler request {request}");
            }
            else if (successor != null)
            {
                successor.HandleRequest(request);
            }
        }
    }
    internal class ConcreteHandlerBar : Handler
    {
        public ConcreteHandlerBar(Handler handler):base(handler)
        {

        }
        internal override void HandleRequest(int request)
        {
            if (request >= 10 && request < 20)
            {
                Console.WriteLine($"{this.GetType().Name} handler request {request}");
            }
            else if (successor != null)
            {
                successor.HandleRequest(request);
            }
        }
    }
    internal class ConcreteHandlerBaz : Handler
    {
        public ConcreteHandlerBaz(Handler handler):base(handler)
        {

        }
        internal override void HandleRequest(int request)
        {
            if (request >= 20 && request < 30)
            {
                Console.WriteLine($"{this.GetType().Name} handler request {request}");
            }
            else if (successor != null)
            {
                successor.HandleRequest(request);
            }
        }
    }
    internal class ConcreteHandlerQux : Handler
    {
        public ConcreteHandlerQux(Handler handler):base(handler)
        {

        }
        internal override void HandleRequest(int request)
        {
            if (request >= 30 && request < 40)
            {
                Console.WriteLine($"{this.GetType().Name} handler request {request}");
            }
            else if (successor != null)
            {
                successor.HandleRequest(request);
            }
        }
    }
}
