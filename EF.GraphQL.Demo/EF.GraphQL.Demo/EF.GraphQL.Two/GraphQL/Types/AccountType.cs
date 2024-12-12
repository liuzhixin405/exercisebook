using EF.GraphQL.Two.Models;
using GraphQL.Types;

namespace EF.GraphQL.Two.GraphQL.Types
{
    public class AccountType:ObjectGraphType<Account>
    {
        public AccountType()
        {
          Field(x => x.Id, type: typeof(IdGraphType));
          Field(x => x.Description);
          Field(x => x.PersonId, type: typeof(IdGraphType));
          Field<AccountTypeEnumType>("Type","Account类型对象的枚举");
        }
    }
}
