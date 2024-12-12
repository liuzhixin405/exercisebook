using EF.GraphQL.Two.Models;
using GraphQL.Types;

namespace EF.GraphQL.Two.GraphQL.Types
{
    public class AccountTypeEnumType:EnumerationGraphType<TypeOfAccount>
    {
        public AccountTypeEnumType()
        {
            Name = "Type";
        }
    }
}
