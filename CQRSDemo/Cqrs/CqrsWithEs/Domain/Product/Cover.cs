namespace CqrsWithEs.Domain.Product
{
    public class Cover
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public Cover(Guid id,string code,string name)
        {
            Id = id;
            Code = code;
            Name = name;
        }
        protected Cover() { }
    }
}
