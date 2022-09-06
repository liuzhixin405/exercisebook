namespace Strategy02
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Context1 context1 = new();
            Context2 context2 = new();
            var res = context1.Result;
            var res2 = context2.Result;
        }


        public interface IStrategy
        {
            string Invoke();
        }
        public interface IContext
        {
            public string Result { get; }
        }
        public class StrategyFoo : IStrategy
        {
            protected string message { get => "foo"; }

            public string Invoke()
            {
                return message;
            }
        }
        public class StrategyBar : IStrategy
        {
            protected string message { get => "bar"; }

            public string Invoke()
            {
                return message;
            }
        }
        public abstract class ContextBase<T> : IContext where T : IStrategy, new()
        {
            protected IStrategy strategy => new T();
            public string Result { get => strategy.Invoke(); }
        }

        public class Context1 : ContextBase<StrategyFoo>
        {

        }
        public class Context2 : ContextBase<StrategyBar> { }
    }
}