
using webapi.Const;
using webapi.Exceptions;
using webapi.Helper;
using webapi.Services;

namespace webapi.BackService
{
    public class SpotTimerTaskDepthAdjust : BackgroundService
    {
        private readonly IBqSpotArbitrageSettingService _bqSpotArbitrageSettingServiceImpl;
        private readonly IBqSpotMarketCoinPairService _bqSpotMarkCoinPairService;
        private readonly IConfiguration _configuration;
        public SpotTimerTaskDepthAdjust(IConfiguration configuration,
            IBqSpotArbitrageSettingService bqSpotArbitrageSettingServiceImpl       
            , IBqSpotMarketCoinPairService bqSpotMarkCoinPairService)
        {
            _bqSpotMarkCoinPairService = bqSpotMarkCoinPairService;
            _bqSpotArbitrageSettingServiceImpl = bqSpotArbitrageSettingServiceImpl;
            _configuration = configuration;
        }

        public static List<string> marketArray = new();
        private static readonly int DEPTH_LEVEL = 40;
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
           if(marketArray.Count == 0)
            {
                string contracts = _configuration["spot.markets"]??throw new ParamsErrorException("配置文件有错");
                marketArray = contracts.Split(',').ToList();
            }
            List<BqSpotMarketCoinPair> coinPairs = CacheHelper.Get<List<BqSpotMarketCoinPair>>(BTCQMemecacheKeys.SPOTCOINPAIRLIST);
            return Task.CompletedTask;
        }
    }
}
