using Cat.Seckill.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cat.Seckill.Base.EFCore.Repository
{
    public class UserRepository : ReposioryBase<Account>
    {
        public UserRepository(SeckillDbContext dbContext) : base(dbContext)
        {
        }
    }
}
