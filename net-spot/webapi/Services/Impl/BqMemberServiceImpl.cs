using DapperDal;
using webapi.Dals.Impls;

namespace webapi.Services.Impl
{
    public class BQMemberServiceImpl : BqBaseService<BqMemberDalImpl>, IBqMemberService
    {
        public BQMemberServiceImpl(IConfiguration configuration) : base(configuration)
        {
        }

        protected override BqMemberDalImpl CreateInstance(string config)
        {
            return new BqMemberDalImpl(config);
        }

        public List<BqMember> FindListByemail(string emiEmail)
        {
            return instance.GetList(x => x.FUserEmail == emiEmail).ToList();
        }
        public BqMember FindByemail(string emiEmail)
        {
            return instance.GetFirst(x => x.FUserEmail == emiEmail);
        }


    }
}
