using GraphQL.Types;

namespace EF.GraphQL.Two.GraphQL.Types
{
    public class PersonInputType:InputObjectGraphType
    {
        public PersonInputType()
        {
            Name = "personInput";
            Field<NonNullGraphType<StringGraphType>>("name");
            Field<NonNullGraphType<StringGraphType>>("address");
        }
    }
}
