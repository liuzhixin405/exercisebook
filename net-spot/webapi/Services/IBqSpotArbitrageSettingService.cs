namespace webapi.Services
{
    public interface IBqSpotArbitrageSettingService
    {
        BqSpotArbitrageSetting getArbitrageSettingByCode(string code);
        BqSpotArbitrageSetting readArbitrageSettingByCode(string code);
        void Save(BqSpotArbitrageSetting ba);
        void Update(BqSpotArbitrageSetting setting);
    }
}