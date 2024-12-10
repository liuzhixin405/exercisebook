namespace EF.GraphQL.Demo
{
    public class Market
    {
        public int Id { get; set; }
        public string CoinType { get; set; }
        public decimal Price { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
