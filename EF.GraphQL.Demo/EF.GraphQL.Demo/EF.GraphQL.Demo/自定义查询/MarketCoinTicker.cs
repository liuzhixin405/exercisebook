namespace EF.GraphQL.Demo
{
    public class MarketCoinTicker
    {
        public int Id { get; set; }
        public string CoinType { get; set; }
        public decimal Price { get; set; }
        public int TraderId { get; set; }  // 外键，关联到 Trader 表
        public int MarketId { get; set; }  // 外键，关联到 Market 表
        public DateTime LastUpdated { get; set; }
    }
}
