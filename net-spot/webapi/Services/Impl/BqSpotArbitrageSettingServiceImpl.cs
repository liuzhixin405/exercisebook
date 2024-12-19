using webapi.Dals.Impls;

namespace webapi.Services.Impl
{
    public class BqSpotArbitrageSettingServiceImpl : BqBaseService<BqSpotArbitrageSettingDalImpl>, IBqSpotArbitrageSettingService
    {
        public BqSpotArbitrageSettingServiceImpl(IConfiguration configuration) : base(configuration)
        {
        }

        protected override BqSpotArbitrageSettingDalImpl CreateInstance(string config)
        {
            return new BqSpotArbitrageSettingDalImpl(config);
        }

        public Task<BqSpotArbitrageSetting> getArbitrageSettingByCode(String code)
        {
            return instance.GetFirst(x => x.FCode == code);
        }


        public Task<BqSpotArbitrageSetting> readArbitrageSettingByCode(String code)
        {
            return instance.GetFirst(x => x.FCode == code);
        }


        public Task Save(BqSpotArbitrageSetting ba)
        {
           return instance.Insert(ba);
        }


        public Task Update(BqSpotArbitrageSetting setting)
        {
           return instance.Update(setting);
        }
    }
}
