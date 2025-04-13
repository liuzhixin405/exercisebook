using spot.Domain.Accounts.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace spot.Application.Interfaces.Repositories.Accounts
{
    public interface IAccountRepository : IGenericRepository<Account>
    {
        Task<IReadOnlyList<Account>> GetAccountsByUserIdAsync(string userId);
        Task<Account> GetAccountByUserIdAndCurrencyAsync(string userId, string currency);
        Task<bool> UpdateBalanceAsync(string userId, string currency, decimal availableDelta, decimal holdDelta);
        Task<bool> TransferAsync(string fromUserId, string toUserId, string currency, decimal amount);
    }
}