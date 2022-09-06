using EF.Core;
using IBusiness;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class UserService : IUserService
    {
        [CacheData]      //标记了的被读取
        public User AddUser(User user)
        {
            Console.WriteLine("用户添加成功");
            return user;
        }

        public void Print()
        {
            Console.WriteLine("print abc");
        }
    }
}
