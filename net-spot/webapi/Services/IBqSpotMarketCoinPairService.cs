namespace webapi.Services
{
    public interface IBqSpotMarketCoinPairService
    {
        bool AddMarketCoinPair(BqSpotMarketCoinPair coinPair);
        void createTable(string coinPairName);
        long Time();
        void Update(BqSpotMarketCoinPair coinPair);
    }
}