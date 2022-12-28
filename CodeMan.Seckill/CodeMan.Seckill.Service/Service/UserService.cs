using CodeMan.Seckill.Entities.Models;
using CodeMan.Seckill.Service.Repository;

namespace CodeMan.Seckill.Service.service
{
    public class UserService : IUserService
    {

        private readonly IRepositoryWrapper _repositoryWrapper;

        public UserService(IRepositoryWrapper repositoryWrapper)
        {
            _repositoryWrapper = repositoryWrapper;
        }

        public Account GetUserById(int userid)
        {
            return _repositoryWrapper.User.GetUserById(userid);
        }
    }
}