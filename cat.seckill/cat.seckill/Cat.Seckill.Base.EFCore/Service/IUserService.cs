using Cat.Seckill.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cat.Seckill.Base.EFCore.Service
{
    public interface IUserService
    {
        Task<bool> CreateUser(string username, string password, string phone, string email);
        Task<Account>  GetUserById(int id);
    }
}
