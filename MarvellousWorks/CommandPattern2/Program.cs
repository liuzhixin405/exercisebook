namespace CommandPattern2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Invoker invoker = new Invoker();
            invoker.AddHandler(new Receiver1().A);
            invoker.AddHandler(new Receiver2().B);
            invoker.AddHandler(Receiver3.C);
            invoker.Run();

        }
        public delegate void VoidHandler();
        public class Receiver1 { public void A() { } }
        public class Receiver2 { public void B() { } }
        public class Receiver3 { public static void C() { } }
        public class Invoker
        {
            IList<VoidHandler> handlers = new List<VoidHandler>();
            public void AddHandler(VoidHandler voidHandler)
            {
                handlers.Add(voidHandler);
            }
            public void Run()
            {
                foreach (var handler in handlers)
                {
                    handler();
                }
            }
        }
    }
}