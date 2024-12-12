using EF.GraphQL.Two.GraphQL.Types;
using EF.GraphQL.Two.Interfaces;
using EF.GraphQL.Two.Models;
using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;

namespace EF.GraphQL.Two.GraphQL.Queries
{
    public class AppQuery : ObjectGraphType
    {
        public AppQuery(IPersonRepository personRepository)
        {
            // 使用 AddField 来添加查询字段
            AddField(new FieldType
            {
                Name = "persons",  // 字段名称
                Type = typeof(ListGraphType<PersonType>),  // 返回类型
                Resolver = new FuncFieldResolver<List<Person>>(async context =>
                {
                    // 异步解析器：调用异步方法获取数据
                    var result = await personRepository.GetPersons();
                    return result.ToList();
                })
            });

            //Field<PersonType>("person", arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "personId" }),
            //        resolve: context =>
            //        {
            //            var personId = context.GetArgument<string>("personId");
            //            if (Guid.TryParse(personId, out Guid id)) return  personRepository.GetById(id);
            //            context.Errors.Add(new ExecutionError("Invalid personId"));
            //            return null;


            //        });

            AddField(new FieldType
            {
                Name = "person",
                Type = typeof(PersonType),
                Arguments = new QueryArguments(new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "personId" }),
                Resolver = new FuncFieldResolver<Person>(async context =>
                {
                    var personId = context.GetArgument<string>("personId");
                    if (Guid.TryParse(personId, out Guid id))
                    {
                        var result = await personRepository.GetById(id);
                        return result;
                    }
                    context.Errors.Add(new ExecutionError("Invalid personId"));
                    return null;
                })
            });
        }
    }
}
