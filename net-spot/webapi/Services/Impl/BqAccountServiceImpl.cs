using System.Runtime.InteropServices;
using webapi.Dals.Impls;
using webapi.Exceptions;

namespace webapi.Services.Impl
{
    public class BQAccountServiceImpl : BqBaseService<BqAccountDalImpl>, IBqAccountService
    {

        public BQAccountServiceImpl(IConfiguration configuration) : base(configuration)
        {

        }
        protected override BqAccountDalImpl CreateInstance(string config)
        {
            return new BqAccountDalImpl(config);
        }

        public bool UpdateAmount(int uid, int currencyId, decimal amount, decimal frozenAmount)
        {
            var account = instance.GetFirst(x => x.FUserId == uid && x.CurrencyTypeId == currencyId && x.BtcLocked >= 0 && x.Btc >= 0);
            if (account == null)
            {
                throw new ParamsErrorException($"id={uid} currrenctId={currencyId} 的用户不存在或余额不足");
            }
            account.Btc = amount;
            account.BtcLocked = frozenAmount;
            account.Lastupdate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var result = instance.Update(account);
            return result;

        }

        public BqAccount GetByUserAndCurrency(long uid, long cid)
        {
            var result = instance.GetFirst(x => x.FUserId == uid && x.CurrencyTypeId == cid);
            return result;
        }

        public void InsertAccount(int uid, string email, int currencyId)
        {
            instance.Insert(new BqAccount
            {
                Btc = 0m,
                BtcLocked = 0m,
                FUserId = uid,
                FUserEmail = email,
                Lastupdate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                CurrencyTypeId = currencyId
            });
        }


    }
}
