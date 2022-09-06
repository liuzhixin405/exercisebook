using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationCenter.Models
{
    public class CurrentUserModel:BaseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Account { get; set; }
        public string Mobile { get; set; }
        public string EMail { get; set; }
        //public string Password { get; set; }
        public string Role { get; set; }
        public int Age { get; set; }
        /// <summary>
        /// 0女 1男
        /// </summary>
        public byte Sex { get; set; }
    }
}
