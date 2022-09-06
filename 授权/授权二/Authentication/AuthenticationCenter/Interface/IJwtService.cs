using AuthenticationCenter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationCenter.Interface
{
    public interface IJwtService
    {
        string GetToken(CurrentUserModel userModel);
    }
}
