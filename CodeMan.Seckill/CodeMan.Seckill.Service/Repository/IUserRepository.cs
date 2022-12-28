using System.Collections.Generic;
using CodeMan.Seckill.Entities.Models;

namespace CodeMan.Seckill.Service.Repository
{
    public interface IUserRepository : IRepositoryBase<Account>
    {
        IEnumerable<Account> GetAllUsers();

        Account GetUserById(int userid);

        void CreateUser(Account user);
    }
}