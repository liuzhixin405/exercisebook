using DapperDal;

namespace webapi.Dals.Impls
{
    public class BqMemberDalImpl:DalBase<BqMember,int>
    {
        public BqMemberDalImpl(string configuration) : base(configuration)
        {

        }
    }
}
