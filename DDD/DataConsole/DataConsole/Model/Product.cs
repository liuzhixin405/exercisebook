namespace DataConsole.Model
{
    public class Product
    {
        public bool IsDiscontinued { get; internal set; }
        public decimal UnitsInStock { get; internal set; }
        public int Id { get; internal set; }
        public Category Category { get; internal set; }
    }

    public class Category
    {
        public int Id { get; set; }
    }
}
