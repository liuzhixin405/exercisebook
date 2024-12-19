namespace webapi.Services
{
    public interface IBqSpotArbitrageSettingService
    {
        Task<BqSpotArbitrageSetting> getArbitrageSettingByCode(string code);
        Task<BqSpotArbitrageSetting> readArbitrageSettingByCode(string code);
        Task Save(BqSpotArbitrageSetting ba);
        Task Update(BqSpotArbitrageSetting setting);
    }
}