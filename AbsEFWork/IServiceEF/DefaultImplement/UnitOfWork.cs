using EntityEF.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IServiceEF.DefaultImplement
{
    public class UnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
    {
        private readonly ConcurrentDictionary<Type, object> _repositories;
        private readonly TContext _context;
        private readonly IServiceProvider _serviceProvider;
        public UnitOfWork(TContext context, IServiceProvider serviceProvider)
        {
            _repositories = new ConcurrentDictionary<Type, object>();
            _context = context;
            _serviceProvider = serviceProvider;
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        public IBaseRepository<TEntity> GetRepository<TEntity>()
            where TEntity : class, IEntity         
        {
            var type = typeof(TEntity);
            if (!_repositories.ContainsKey(type))
            {
                // 通过 IServiceProvider 解析仓储实现
                var repositoryInstance = _serviceProvider.GetService(typeof(IBaseRepository<TEntity>));

                if (repositoryInstance == null)
                {
                    // 如果没有找到，使用默认实现
                    repositoryInstance = new DefaultEfCoreRepository<TEntity,TContext>(_context);
                }

                // 将创建的实例存储在缓存中
                _repositories[type] = repositoryInstance;
            }

            return (IBaseRepository<TEntity>)_repositories[type];
        }

        public async Task<int> SaveChangesAsync()
        {
            
            return await _context.SaveChangesAsync();
        }

    }
}
