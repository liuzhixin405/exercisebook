namespace EF.GraphQL.Two.Models
{
    public class Account
    {
        public Guid Id { get; set; }
        public TypeOfAccount Type { get; set; }
        public string Description { get; set; }

        public Guid PersonId { get; set; }
        public Person Person { get; set; }

    }

    public enum TypeOfAccount
    {
        Invalid,
        Free,
        Junior,
        Intermediate
    }
}
