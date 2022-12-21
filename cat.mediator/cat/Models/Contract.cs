namespace cat.Models
{
    public class Contract
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }

        public static Contract CreateNew(string name)
        {
            return new Contract { Name = name ,CreateTime = DateTime.Now};
        }
        private Contract()
        {

        }
    }
}
