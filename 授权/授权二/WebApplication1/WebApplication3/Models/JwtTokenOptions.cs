using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication3.Models
{
    public class JwtTokenOptions
    {
        public string Audience { get; set; }
        public string SecurityKey { get; set; }
        public string Issuer { get; set; }
    }
}
