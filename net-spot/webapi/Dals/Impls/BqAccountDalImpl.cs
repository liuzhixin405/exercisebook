using DapperDal;

namespace webapi.Dals.Impls
{
    public class BqAccountDalImpl : DalBase<BqAccount,long>
    {
        public BqAccountDalImpl(string conn):base(conn)
        {
            
        }
    }
}
