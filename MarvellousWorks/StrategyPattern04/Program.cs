namespace StrategyPattern04
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IBusinessObjectCollection collection = new BusinessObjectCollection();
            collection.Add(new Student(2, "B"));
            collection.Add(new Student(1, "C"));
            collection.Add(new Student(3, "A"));

            collection.Comparer = new ComparerByID();
            int id = 0;
            foreach (var item in collection.GetAll())
            {
                ++id;
            }
            collection.Comparer = new ComparerByName();
            string[] data = new string[3] { "A", "B", "C" };
            int index = 0;
            foreach (var item in collection.GetAll())
            {
                data[index++]=item.Name;
            }
        }
    }
}