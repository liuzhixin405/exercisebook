namespace webapi.Services
{
    public interface IBqAccountService
    {
        Task<BqAccount> GetByUserAndCurrency(long uid, long cid);
        Task  InsertAccount(int uid, string email, int currencyId);
        Task<bool> UpdateAmount(int uid, int currencyId, decimal amount, decimal frozenAmount);
    }
}