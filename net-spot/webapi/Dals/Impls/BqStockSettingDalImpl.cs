using DapperDal;

namespace webapi.Dals.Impls
{
    public class BqStockSettingDalImpl:DalBase<BqStockSetting,int>
    {
        public BqStockSettingDalImpl(string connString):base(connString)
        {
            
        }
    }
}
