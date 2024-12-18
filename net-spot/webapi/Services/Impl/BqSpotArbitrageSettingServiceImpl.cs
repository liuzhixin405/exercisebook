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

        public BqSpotArbitrageSetting getArbitrageSettingByCode(String code)
        {
            return instance.GetFirst(x => x.FCode == code);
        }


        public BqSpotArbitrageSetting readArbitrageSettingByCode(String code)
        {
            return instance.GetFirst(x => x.FCode == code);
        }


        public void Save(BqSpotArbitrageSetting ba)
        {
            instance.Insert(ba);
        }


        public void Update(BqSpotArbitrageSetting setting)
        {
            instance.Update(setting);
        }
    }
}
