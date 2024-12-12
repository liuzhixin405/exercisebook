using EF.GraphQL.Two.GraphQL.Types;
using EF.GraphQL.Two.Interfaces;
using EF.GraphQL.Two.Models;
using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;

namespace EF.GraphQL.Two.GraphQL.Queries
{
    public class AppMutation:ObjectGraphType
    {
        public AppMutation(IPersonRepository personRepository)
        {
            AddField(new FieldType
            {
                Name = "createPerson",
                Type = typeof(PersonType),
                Arguments = new QueryArguments(new QueryArgument<NonNullGraphType<PersonInputType>> { Name = "person" }),
                Resolver = new FuncFieldResolver<Person>(async context =>
                {
                    var person = context.GetArgument<Person>("person");
                        return await personRepository.CreatePerson(person);
                })
            });

            AddField(new FieldType
            {
                Name = "updatePerson",
                Type = typeof(PersonType),
                Arguments = new QueryArguments(new QueryArgument<NonNullGraphType<PersonInputType>> { Name = "person" },
                    new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "personId" }),
                Resolver = new FuncFieldResolver<Person>(async context =>
                {
                    var personId = context.GetArgument<Guid>("personId");
                    var person = context.GetArgument<Person>("person");

                    var entitiy = await personRepository.GetById(personId);
                    if (entitiy == null)
                    {
                        context.Errors.Add(new ExecutionError("Person not found"));
                        return null;
                    }

                    return await personRepository.UpdatePerson(entitiy, person);
                })
            });

            AddField(new FieldType
            {
                Name = "deletePerson",
                Type = typeof(StringGraphType),
                Arguments = new QueryArguments(new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "personId" }),
                Resolver = new FuncFieldResolver<string>(async context =>
                {
                    var personId = context.GetArgument<Guid>("personId");
                   
                    var entitiy = await personRepository.GetById(personId);
                    if (entitiy == null)
                    {
                        context.Errors.Add(new ExecutionError("Person not found"));
                        return null;
                    }

                     await personRepository.DeletePerson(entitiy);
                    return  $"deleted person with id {personId}";
                })
            });
        }
    }
}
