using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Play.Common.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Play.Common.MongoDB
{
    public static class Extensions
    {
        public static IServiceCollection AddMongo(this IServiceCollection services)
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));
            services.AddSingleton(serviceProvider =>
            {
                var configuration = serviceProvider.GetService<IConfiguration>();
                var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                var mongoDbSettings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
                var mongoClient = new MongoClient(mongoDbSettings.ConenctionString);
                return mongoClient.GetDatabase(serviceSettings.ServiceName);
            });
            return services;
        }
        public static IServiceCollection AddMongoRepository<T>(this IServiceCollection services,string collectionName)where T:IEntity
        {
            services.AddSingleton<IRepository<T>>(serviceProvider =>
            {
                var db = serviceProvider.GetService<IMongoDatabase>();
                return new MongoRepository<T>(db,collectionName);
            });
            return services;
        }
    }
}
