using AutoMapper;
using BackStageDemo.Application.User.Dto;
using BackStageDemo.Domain.UserInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackStageDemo.Application
{
    public class BackStageDemoApplicationAutoMapperProfile:Profile
    {
        public BackStageDemoApplicationAutoMapperProfile()
        {
            CreateMap<Users, UserDto>();
            //CreateMap<Users, UserDto>().ReverseMap();  //反向转换
        }
    }
}
