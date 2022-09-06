using EF.Core;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBusiness
{
    public interface IUserService
    {
        User AddUser(User user);
        void Print();
    }
}
