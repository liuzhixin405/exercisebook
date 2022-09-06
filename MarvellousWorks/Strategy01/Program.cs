namespace Strategy01
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int[] arr = new int[] { 1, 2, 3, 4 };
            IStrategy first = new First();
            IStrategy last = new Last();

            Context context = new Context();
            context.Strategy = first;

            var res = context.GetData(arr);
            context.Strategy = last;
            res = context.GetData(arr);
        }
    }

    public interface IStrategy
    {
        int PickUp(int[] data);
    }
    public interface IContext
    {
        IStrategy Strategy { get; set; }
        int GetData(int[] data);
    }

    public class First : IStrategy
    {
        public int PickUp(int[] data)
        {
            Array.Sort(data);
            return data[0];
        }
    }

    public class Last : IStrategy
    {
        public int PickUp(int[] data)
        {
            Array.Sort(data);
            return data[data.Length - 1];
        }
    }

    public class Context : IContext
    {
        protected IStrategy strategy;
        public IStrategy Strategy { get => strategy; set => strategy = value; }

        public int GetData(int[] data)
        {
            return strategy.PickUp(data);
        }
    }
}