namespace webapi.Services
{
    public interface IBqSpotMarketCoinPairService
    {
        Task<bool> AddMarketCoinPair(BqSpotMarketCoinPair coinPair);
        Task createTable(string coinPairName);
        Task<long> Time();
        Task Update(BqSpotMarketCoinPair coinPair);
    }
}