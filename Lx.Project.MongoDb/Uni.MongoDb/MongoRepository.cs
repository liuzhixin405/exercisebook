using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Uni.MongoDb
{
    public abstract class MongoRepository<T> where T : BaseEntity
    {
        protected readonly IMongoCollection<T> _collection;
        public abstract MongoClient Client { get; }
        public abstract string DbName { get; }
        public MongoRepository()
        {
            var database = Client.GetDatabase(DbName);
            _collection = database.GetCollection<T>(typeof(T).Name);
        }

        public IMongoCollection<T> Collection
        {
            get
            {
                return this._collection;
            }
        }


        public TL TransactionAsync<TL>(Func<IClientSessionHandle, TL> func)
        {
            TL res = default;
            var options = new ClientSessionOptions { CausalConsistency = true };
            using var session = Client.StartSession(options);
            try
            {
                session.StartTransaction();
                if (func != null)
                {
                    res = func.Invoke(session);
                }
                var t = (res as Task);

                //session 中代码报错会自动 abort 当前 Transaction
                //t.ContinueWith(user => { session.AbortTransaction(); },TaskContinuationOptions.NotOnRanToCompletion);

                t?.Wait();
                session.CommitTransaction();
            }
            catch (Exception ex)
            {
                session.AbortTransaction();
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

        public async Task<bool> UpdateAsyncById(string SelectName, string values, string Id, IClientSessionHandle session = null)
        {
            try
            {

                UpdateDefinition<T> updateDefinition = Builders<T>.Update.Set(SelectName, values);
                UpdateResult updateResult = await _collection.UpdateOneAsync(x => x._id == Id, updateDefinition); // replaces first match
                return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region FindListByPageAsync 异步分页查询集合
        /// <summary>
        /// 异步分页查询集合
        /// </summary>
        /// <param name="expression">mongodb连接信息</param>
        /// <param name="filter">查询条件</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="field">要查询的字段,不写时查询全部</param>
        /// <param name="orderByParams">要排序的字段</param>
        /// <returns></returns>
        public async Task<Tuple<long, List<T>>> FindListByPageAsync(Expression<Func<T, bool>> expression, int pageIndex, int pageSize, string[] field = null, List<OrderByParam> orderByParams = null, IClientSessionHandle session = null, bool isPage = true)
        {
            try
            {
                SortDefinition<T> sort = null;
                //根据时间排序
                //var sort = Builders<T>.Sort.Ascending("State").Descending("");
                if (orderByParams?.Count > 0)
                {
                    sort = Builders<T>.Sort.Combine();
                    orderByParams.ForEach(action: x =>
                    {
                        if (x.IsAscending)
                        {
                            sort = sort.Ascending(x.FieldName);
                        }
                        else
                        {
                            sort = sort.Descending(x.FieldName);
                        }
                    });
                    //  sort =Builders<T>.Sort.Descending("CreatedOnUtc").Ascending("Width");
                }

                var total = session == null
                 ? await _collection.CountDocumentsAsync(expression)
                 : await _collection.CountDocumentsAsync(session, expression);




                //不指定查询字段
                if (field == null || field.Length == 0)
                {
                    if (isPage)
                    {
                        if (sort == null)
                        {
                            return Tuple.Create(total, (session == null ? await _collection.Find(expression).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToListAsync() : await _collection.Find(session, expression).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToListAsync()));
                        }
                        //进行排序
                        return Tuple.Create(total, (session == null ? await _collection.Find(expression).Sort(sort).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToListAsync() : await _collection.Find(session, expression).Sort(sort).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToListAsync()));
                    }
                    else
                    {
                        if (sort == null)
                        {
                            return Tuple.Create(total, (session == null ? await _collection.Find(expression).ToListAsync() : await _collection.Find(session, expression).ToListAsync()));
                        }
                        //进行排序
                        return Tuple.Create(total, (session == null ? await _collection.Find(expression).Sort(sort).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToListAsync() : await _collection.Find(session, expression).Sort(sort).ToListAsync()));
                    }
                }

                //制定查询字段
                var fieldList = new List<ProjectionDefinition<T>>();
                for (int i = 0; i < field.Length; i++)
                {
                    fieldList.Add(Builders<T>.Projection.Include(field[i].ToString()));
                }
                var projection = Builders<T>.Projection.Combine(fieldList);
                fieldList?.Clear();

                if (isPage)
                {
                    //不排序
                    if (sort == null)
                    {
                        return Tuple.Create(total, (session == null ? await _collection.Find(expression).Project<T>(projection).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToListAsync() : await _collection.Find(session, expression).Project<T>(projection).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToListAsync()));
                    }
                    //排序查询
                    return Tuple.Create(total, (session == null ? await _collection.Find(expression).Sort(sort).Project<T>(projection).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToListAsync() : await _collection.Find(session, expression).Sort(sort).Project<T>(projection).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToListAsync()));
                }
                else
                {
                    //不排序
                    if (sort == null)
                    {
                        return Tuple.Create(total, (session == null ? await _collection.Find(expression).Project<T>(projection).ToListAsync() : await _collection.Find(session, expression).Project<T>(projection).ToListAsync()));
                    }
                    //排序查询
                    return Tuple.Create(total, (session == null ? await _collection.Find(expression).Sort(sort).Project<T>(projection).ToListAsync() : await _collection.Find(session, expression).Sort(sort).Project<T>(projection).ToListAsync()));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

    }
}
