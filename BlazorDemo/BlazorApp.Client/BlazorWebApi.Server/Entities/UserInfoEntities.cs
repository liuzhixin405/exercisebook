using BlazorApp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWebApi.Server.Entities
{
    public class UserInfoEntities
    {
        public List<UserInfo> UserInfos;
        public UserInfoEntities()
        {
			UserInfos = new List<UserInfo>();
            for (int i = 0; i < 100; i++)
            {
				UserInfo userinfo = new UserInfo();
				UserInfos.Add(userinfo);
				userinfo.Address = "中华人民共和国" + i;
				userinfo.Age = i;
				userinfo.BirthDate = DateTime.UtcNow;
				
				userinfo.UserID = (i + 1);
				userinfo.UserName = "无名氏" + i;
				userinfo.DeptId = 1;
				if (i > 10)
				{
					if (i % 2 == 0)
                    {
						userinfo.Gender = Gender.Male;
						userinfo.DeptId = 2;
                    }
                    else
                    {
						userinfo.Gender = Gender.Female;
						userinfo.DeptId = 3;
					}
					
                }
			}
        }
    }
}
