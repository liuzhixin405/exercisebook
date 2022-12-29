using Cat.Seckill.Entities.BaseRepository;
using Cat.Seckill.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cat.Seckill.Base.EFCore.Service
{
    public class UserService : IUserService
    {
        private readonly IRepository<Account> repository;
        public UserService(IRepository<Account> repository)
        {
            this.repository = repository;
        }
        public async Task<bool> CreateUser(string username, string password, string phone, string email)
        {
            await repository.Create(new Account
            {
                Password = password,
                UserName = username,
                Phone = phone,
                Email = email
            });
            var query = await repository.FindByCondition(x => x.UserName.Equals(username));
            return query.Any();
        }

        public async Task<Account> GetUserById(int id)
        {
            return await repository.GetById(id);
        }
    }
}
