using EF.GraphQL.Two.Interfaces;
using EF.GraphQL.Two.Models;
using EF.GraphQL.Two.Models.Context;

namespace EF.GraphQL.Two.Repository
{
    public class AccountRepository:IAccountRepository
    {
        private readonly SampleContext _sampleContext;
        public AccountRepository(SampleContext sampleContext)
        {
            _sampleContext = sampleContext;
        }

        public IEnumerable<Account> GetAccounts(Guid personId)
        {
            return _sampleContext.Accounts.Where(a => a.PersonId == personId);
        }
    }
}
