using MongoDB.BooksApi.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MongoDB.BooksApi.Repository
{
    public class BookRepository<T> : MongoRepository<T> where T : BaseEntity
    {
        public override MongoClient Client => new MongoClient(_mongoDbConnectionString);

        public override string DbName => "Mongo";
        private readonly string _mongoDbConnectionString = "mongodb://localhost:27017";
    }
}
