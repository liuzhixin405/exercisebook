using BackStageDemo.Application.User.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace BackStageDemo.Application.User
{
    public interface IUserAppService:IApplicationService
    {
        List<UserDto> GetAll();
    }
}
