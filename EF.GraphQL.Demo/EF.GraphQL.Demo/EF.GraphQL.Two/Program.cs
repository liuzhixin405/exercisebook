using EF.GraphQL.Two.GraphQL.Queries;
using EF.GraphQL.Two.GraphQL.Schemas;
using EF.GraphQL.Two.GraphQL.Types;
using EF.GraphQL.Two.Interfaces;
using EF.GraphQL.Two.Models.Context;
using EF.GraphQL.Two.Repository;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;

namespace EF.GraphQL.Two
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddSingleton<SampleContext>();
            builder.Services.AddTransient<IPersonRepository, PersonRepository>();
            builder.Services.AddTransient<IAccountRepository, AccountRepository>();

            // ע�����ݿ������ġ��ֿ������
            builder.Services.AddTransient<AppQuery>();  // ע���ѯ��
            builder.Services.AddTransient<AppMutation>();
            builder.Services.AddTransient<PersonType>(); // ע�� Person ����
            builder.Services.AddTransient<AppSchema>(); // ע�� GraphQL Schema

            // ע�� GraphQL ����
            builder.Services.AddGraphQL(options =>
            {
                options

                .AddSystemTextJson()
                .AddSchema<AppSchema>()
                .AddGraphTypes(typeof(AppSchema).Assembly);
            });
          
            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseGraphQL<ISchema>();
            app.UseGraphQLPlayground();

            app.MapControllers();

            app.Run();
        }
    }
}
