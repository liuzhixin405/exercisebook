using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Architecture.Domain.Customers
{
    public class Admin
    {
        public static Admin CreateNew(string name)
        {
            var admin = new Admin() { Name= name };
            return admin;
        }
        protected Admin() {
            Name = string.Empty;
        }
        public string Name { get; private set; }
        public string PasswordHash { get; private set; }
    }
}
