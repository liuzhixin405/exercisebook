using DapperDal;

namespace webapi.Dals.Impls
{
    public class BqAccountAmountLogDalImpl(string configuration) : DalBase<BqAccountAmountLog, long>(configuration)
    {
    }
}
