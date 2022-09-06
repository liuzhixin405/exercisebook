using BackStageDemo.Application.User.Dto;
using BackStageDemo.Domain.UserInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace BackStageDemo.Application.User
{
    public class UserAppService:ApplicationService,IUserAppService
    {
        private readonly IRepository<Users> _userRepository;
        public UserAppService(IRepository<Users> userRepository)
        {
            _userRepository = userRepository;
        }
        public List<UserDto> GetAll()
        {
            var users = _userRepository.ToList();
            return ObjectMapper.Map<List<Users>, List<UserDto>>(users);
        }
    }
}
