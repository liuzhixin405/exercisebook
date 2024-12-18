using DapperDal;

namespace webapi.Dals.Impls
{
    public class BqSpotArbitrageSettingDalImpl:DalBase<BqSpotArbitrageSetting, long>
    {
        public BqSpotArbitrageSettingDalImpl(string conn):base(conn)
        {
            
        }
    }
}
