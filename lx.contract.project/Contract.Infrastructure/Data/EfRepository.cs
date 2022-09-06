using Ardalis.Specification.EntityFrameworkCore;
using Contract.Core.Entities;
using Contract.Core.Interfaces;
using Contract.Core.SharedKernel;
using Contract.SharedKernel.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.Infrastructure.Data
{
    public class EfRepository<T> : RepositoryBase<T>, IReadRepository<T>, IRepository<T> where T : class, IAggregateRoot
    {
        public EfRepository(AppDbContext dbContext):base(dbContext)
        {

        }
    }
}
