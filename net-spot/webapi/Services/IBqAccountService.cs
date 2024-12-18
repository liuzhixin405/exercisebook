namespace webapi.Services
{
    public interface IBqAccountService
    {
        BqAccount GetByUserAndCurrency(long uid, long cid);
        void InsertAccount(int uid, string email, int currencyId);
        bool UpdateAmount(int uid, int currencyId, decimal amount, decimal frozenAmount);
    }
}