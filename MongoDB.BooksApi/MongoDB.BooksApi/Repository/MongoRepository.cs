using MongoDB.BooksApi.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MongoDB.BooksApi.Repository
{
    public abstract class MongoRepository<T> where T:BaseEntity
    {
        private readonly IMongoCollection<T> _collection;
        public abstract MongoClient Client { get; }
        public abstract string DbName { get; }

        protected IMongoCollection<T> Collection => _collection;

        public MongoRepository()
        {
            var database = Client.GetDatabase(DbName);
            _collection = database.GetCollection<T>(typeof(T).Name);
        }
        public async  Task<TL> TransactionAsync<TL>(Func<IClientSessionHandle,TL> func)
        {
            TL res = default(TL);
            var options = new ClientSessionOptions { CausalConsistency = true };
            using var session =await Client.StartSessionAsync(options);
            try
            {
                session.StartTransaction();
                if (func != null) res = func.Invoke(session);
                var t = (res as Task);
                t?.Wait();
               await session.CommitTransactionAsync();
            }
            catch(Exception ex)
            {
               await session.AbortTransactionAsync();
                throw ex;
            }
            return res;
        }

        public async Task<Tuple<long, List<T>>> GetListAsyncSpecialHandleTime(Expression<Func<T, bool>> expression, int page, int limit, List<SpecialHandleTimeParam> dateTimeParams = null, IClientSessionHandle session = null)
        {
            try
            {
                var builder = Builders<T>.Filter;
                var filter = builder.Where(expression);//&builder.Gt("CreatedOnUtc", DateTime.SpecifyKind(Convert.ToDateTime("2020/9/9 8:27:40"), DateTimeKind.Utc)) & builder.Lt("CreatedOnUtc", DateTime.SpecifyKind(Convert.ToDateTime("2020/9/9 8:28:40"), DateTimeKind.Utc));
                if (dateTimeParams != null)
                {
                    foreach (var item in dateTimeParams)
                    {
                        switch (item.Symbol)
                        {
                            case ">=":
                                filter &= builder.Gte(item.ColoumName, item.Value);
                                break;
                            case ">":
                                filter &= builder.Gt(item.ColoumName, item.Value);
                                break;
                            case "<=":
                                filter &= builder.Lte(item.ColoumName, item.Value);
                                break;
                            case "<":
                                filter &= builder.Lt(item.ColoumName, item.Value);
                                break;
                            case "=":
                                filter &= builder.Eq(item.ColoumName, item.Value);
                                break;
                            default:
                                break;
                        }

                    }
                }
                var total = session == null
                    ? await _collection.CountDocumentsAsync(filter)
                    : await _collection.CountDocumentsAsync(session, filter);

                var data = session == null
                    ? await _collection.Find(filter).Skip((page - 1) * limit).Limit(limit).ToListAsync()
                    : await _collection.Find(session, filter).Skip((page - 1) * limit).Limit(limit).ToListAsync();
                return Tuple.Create(total, data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Tuple<long, List<T>>> GetListAsync(Expression<Func<T, bool>> expression, int page, int limit, IClientSessionHandle session = null)
        {
            try
            {
                var total = session == null
                    ? await _collection.CountDocumentsAsync(expression)
                    : await _collection.CountDocumentsAsync(session, expression);

                var data = session == null
                    ? await _collection.Find(expression).Skip((page - 1) * limit).Limit(limit).ToListAsync()
                    : await _collection.Find(session, expression).Skip((page - 1) * limit).Limit(limit).ToListAsync();
                return Tuple.Create(total, data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> expression, IClientSessionHandle session = null)
        {
            try
            {
                var data = session == null
                    ? await _collection.Find(expression).ToListAsync()
                    : await _collection.Find(session, expression).ToListAsync();
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 自定义查询字段和值
        /// </summary>
        /// <param name="SelectName">自定义查询的字段名称</param>
        /// <param name="values">自定义查询字段名称的值</param>
        /// <param name="session"></param>
        /// <returns></returns>
        public async Task<T> GetOneAsync(string SelectName, string values, IClientSessionHandle session = null)
        {
            var filter = Builders<T>.Filter.Eq(SelectName, values);
            try
            {
                return session == null
                    ? await _collection.Find(filter).FirstOrDefaultAsync()
                    : await _collection.Find(session, filter).FirstOrDefaultAsync();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<long> GetCountAsync(Expression<Func<T, bool>> expression, IClientSessionHandle session = null)
        {
            try
            {
                var total = session == null
                    ? await _collection.CountDocumentsAsync(expression)
                    : await _collection.CountDocumentsAsync(session, expression);
                return total;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<T> GetOneAsync(string id, IClientSessionHandle session = null)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);
            try
            {
                return session == null
                    ? await _collection.Find(filter).FirstOrDefaultAsync()
                    : await _collection.Find(session, filter).FirstOrDefaultAsync();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<T> GetOneAsync(Expression<Func<T, bool>> expression, IClientSessionHandle session = null)
        {
            try
            {
                return session == null
                    ? await _collection.Find(expression).FirstOrDefaultAsync()
                    : await _collection.Find(session, expression).FirstOrDefaultAsync();


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<T> InsertOneAsync(T item, IClientSessionHandle session = null)
        {
            try
            {
                if (session == null)
                {
                    await _collection.InsertOneAsync(item);
                }
                else
                {
                    await _collection.InsertOneAsync(session, item);
                }
                return item;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> InsertManyAsync(IEnumerable<T> values, IClientSessionHandle session = null)
        {
            try
            {
                if (session == null)
                    await _collection.InsertManyAsync(values).ConfigureAwait(false);
                else
                    await _collection.InsertManyAsync(session, values).ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UpdateOneAsync(T item, IClientSessionHandle session = null)
        {
            var filter = Builders<T>.Filter.Eq("_id", item._id);
            try
            {
                var res = session == null
                    ? await _collection.ReplaceOneAsync(filter, item)
                    : await _collection.ReplaceOneAsync(session, filter, item);

                return res.IsAcknowledged
                       && res.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 自定义删除字段的值
        /// </summary>
        /// <param name="DeleteName">删除字段名称</param>
        /// <param name="Name">删除字段值</param>
        /// <param name="session"></param>
        /// <returns></returns>
        public async Task<bool> RemoveOneAsync(string DeleteName, string Name, IClientSessionHandle session = null)
        {
            try
            {
                DeleteResult res = session == null
                    ? await _collection.DeleteOneAsync(Builders<T>.Filter.Eq(DeleteName, Name))
                    : await _collection.DeleteOneAsync(session, Builders<T>.Filter.Eq(DeleteName, Name));
                return res.IsAcknowledged
                       && res.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> RemoveOneAsync(string id, IClientSessionHandle session = null)
        {
            try
            {
                DeleteResult res = session == null
                    ? await _collection.DeleteOneAsync(Builders<T>.Filter.Eq("_id", id))
                    : await _collection.DeleteOneAsync(session, Builders<T>.Filter.Eq("_id", id));
                return res.IsAcknowledged
                       && res.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> RemoveAsync(Expression<Func<T, bool>> expression, IClientSessionHandle session = null)
        {
            try
            {
                DeleteResult res = session == null
                    ? await _collection.DeleteManyAsync(expression)
                    : await _collection.DeleteManyAsync(session, expression);
                return res.IsAcknowledged
                       && res.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
