using System.Collections.Generic;
using System.Linq;
using CodeMan.Seckill.Entities.Models;
using CodeMan.Seckill.Service.Repository;

namespace CodeMan.Seckill.Base.EntityFrameworkCore.Repository
{
    public class UserRepository : RepositoryBase<Account>, IUserRepository
    {
        public UserRepository(SeckillDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public IEnumerable<Account> GetAllUsers()
        {
            return FindAll()
                .OrderBy(user => user.UserId)
                .ToList();
        }

        public Account GetUserById(int userid)
        {
            return FindByCondition(user => user.UserId == userid)
                .FirstOrDefault();
        }

        public void CreateUser(Account user)
        {
            Create(user);
        }
    }
}
