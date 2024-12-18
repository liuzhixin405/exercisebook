using DapperDal;

namespace webapi.Dals.Impls
{
    public class BqSpotMarketCoinPairDalImpl:DalBase<BqSpotMarketCoinPair,long>
    {
        public BqSpotMarketCoinPairDalImpl(string config):base(config)
        {
            
        }
    }
}
