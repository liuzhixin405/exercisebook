namespace 责任链简单02
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Handler handler = new ChairManHandler(null);
            handler = new ManagerHandler(handler);
            handler = new ChargeHandler(handler);

            var context = new Context { Id = 1, Name = "test", ApplyTime = DateTimeOffset.UtcNow, Holiday = 4 };
            handler.Process(context);
            //check context
        }
    }

    public class Context
    {
        public int Id { get; set; }
        public DateTimeOffset ApplyTime { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public int Holiday { get; set; }
        public bool ApplyPassed { get; set; } = false;
    }

    public abstract class Handler
    {

        protected Handler successor;
        public Handler(Handler successor)
        {
            this.successor = successor;
        }

        public abstract Context HandleRequest(Context context);

        public virtual void Process(Context context)
        {
            var handleContext = HandleRequest(context);
            if (handleContext.ApplyPassed)
            {
                handleContext.ApplyTime = DateTimeOffset.UtcNow;
                handleContext.Description = successor.GetType().Name;
            }
            else
            {
                if (successor != null)
                    successor.Process(context);
            }
        }
    }

    public class ChargeHandler : Handler
    {

        public ChargeHandler(Handler handler) : base(handler)
        {
        }

        public override Context HandleRequest(Context context)
        {
            if (context.Holiday <= 3)
                context.ApplyPassed = true;
            return context;
        }
    }
    public class ManagerHandler : Handler
    {
        public ManagerHandler(Handler handler) : base(handler)
        {
        }

        public override Context HandleRequest(Context context)
        {
            if (context.Holiday > 3 && context.Holiday <= 7)
                context.ApplyPassed = true;
            return context;
        }
    }

    public class ChairManHandler : Handler
    {
        public ChairManHandler(Handler handler) : base(handler)
        {
        }

        public override Context HandleRequest(Context context)
        {
            if (context.Holiday > 7)
                context.ApplyPassed = true;
            return context;
        }
    }
}