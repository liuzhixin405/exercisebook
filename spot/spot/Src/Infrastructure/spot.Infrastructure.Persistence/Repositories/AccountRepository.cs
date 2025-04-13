using Microsoft.EntityFrameworkCore;
using spot.Application.Interfaces.Repositories.Accounts;
using spot.Domain.Accounts.Entities;
using spot.Infrastructure.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace spot.Infrastructure.Persistence.Repositories
{
    public class AccountRepository : GenericRepository<Account>, IAccountRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly DbSet<Account> _accounts;

        public AccountRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
            _accounts = dbContext.Set<Account>();
        }

        public async Task<Account> GetAccountByUserIdAndCurrencyAsync(string userId, string currency)
        {
            return await _accounts
                .Where(a => a.UserId == userId && a.Currency == currency)
                .FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<Account>> GetAccountsByUserIdAsync(string userId)
        {
            return await _accounts
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> TransferAsync(string fromUserId, string toUserId, string currency, decimal amount)
        {
            var fromAccount = await GetAccountByUserIdAndCurrencyAsync(fromUserId, currency);
            var toAccount = await GetAccountByUserIdAndCurrencyAsync(toUserId, currency);

            if (fromAccount == null || toAccount == null || fromAccount.Available < amount)
            {
                return false;
            }

            fromAccount.Available -= amount;
            toAccount.Available += amount;

            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            fromAccount.UpdatedAt = now;
            toAccount.UpdatedAt = now;

            _accounts.Update(fromAccount);
            _accounts.Update(toAccount);

            return true;
        }

        public async Task<bool> UpdateBalanceAsync(string userId, string currency, decimal availableDelta, decimal holdDelta)
        {
            var account = await GetAccountByUserIdAndCurrencyAsync(userId, currency);
            if (account == null)
            {
                return false;
            }

            // Check if operation would result in negative balance
            if (account.Available + availableDelta < 0 || account.Hold + holdDelta < 0)
            {
                return false;
            }

            account.Available += availableDelta;
            account.Hold += holdDelta;
            account.UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            _accounts.Update(account);
            return true;
        }
    }
}