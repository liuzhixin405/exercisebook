using EF.GraphQL.Two.Interfaces;
using EF.GraphQL.Two.Models;
using GraphQL.Types;

namespace EF.GraphQL.Two.GraphQL.Types
{
    public class PersonType:ObjectGraphType<Person>
    {
        public PersonType(IAccountRepository accountRepository)
        {
            Field(x => x.Id, type: typeof(IdGraphType));
            Field(x => x.Name);
            Field(x => x.Address);
            Field<ListGraphType<AccountType>>("accounts", resolve:context => accountRepository.GetAccounts(context.Source.Id));
        }
    }
}
