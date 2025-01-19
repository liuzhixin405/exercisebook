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

        public async Task<bool> UpdateAmount(int uid, int currencyId, decimal amount, decimal frozenAmount)
        {
            var account =await instance.GetFirst(x => x.FUserId == uid && x.CurrencyTypeId == currencyId && x.BtcLocked >= 0 && x.Btc >= 0);
            if (account == null)
            {
                throw new ParamsErrorException($"id={uid} currrenctId={currencyId} 的用户不存在或余额不足");
            }
            account.Btc = amount;
            account.BtcLocked = frozenAmount;
            account.Lastupdate = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var result =await instance.Update(account);
            return result;

        }

        public async Task<BqAccount> GetByUserAndCurrency(long uid, long cid)
        {
            var result =await instance.GetFirst(x => x.FUserId == uid && x.CurrencyTypeId == cid);
            return result;
        }

        public Task InsertAccount(int uid, string email, int currencyId)
        {
           return instance.Insert(new BqAccount
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
