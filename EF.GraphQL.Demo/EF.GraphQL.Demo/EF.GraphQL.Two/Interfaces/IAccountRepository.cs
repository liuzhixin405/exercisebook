using EF.GraphQL.Two.Models;

namespace EF.GraphQL.Two.Interfaces
{
    public interface IAccountRepository
    {
        IEnumerable<Account> GetAccounts(Guid personId);
    }
}
