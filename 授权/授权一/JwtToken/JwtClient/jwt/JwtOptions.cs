using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtClient
{
    public class JwtOptions
    {
        public string Secret{get;set; }
        public int AccessExpireHours{get;set;}
        public int RefreshExpireHours{get;set; }
    }
}
