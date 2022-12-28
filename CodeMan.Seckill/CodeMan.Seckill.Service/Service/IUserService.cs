using CodeMan.Seckill.Entities.Models;

namespace CodeMan.Seckill.Service.service
{
    public interface IUserService
    {
        Account GetUserById(int userid);
    }
}