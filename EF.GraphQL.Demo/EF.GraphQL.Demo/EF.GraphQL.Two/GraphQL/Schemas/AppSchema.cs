using GraphQL;
using EF.GraphQL.Two.GraphQL.Queries;
using GraphQL.Types;

namespace EF.GraphQL.Two.GraphQL.Schemas
{
    public class AppSchema:Schema
    {
        public AppSchema(IServiceProvider sp):base(sp)
        {
            var scope = sp.CreateScope();
            var resolver = scope.ServiceProvider;
            Query = resolver.GetRequiredService<AppQuery>();
            Mutation = resolver.GetRequiredService<AppMutation>();
        }
    }
}
