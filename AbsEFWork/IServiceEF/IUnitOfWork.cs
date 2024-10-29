using EntityEF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IServiceEF
{
    public interface IUnitOfWork: IDisposable
    {
        Task<int> SaveChangesAsync();

        IBaseRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IEntity;
    
    }
}
