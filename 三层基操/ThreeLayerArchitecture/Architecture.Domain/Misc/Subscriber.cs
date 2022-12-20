using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Architecture.Domain.Misc
{
    public class Subscriber
    {
        public static Subscriber CreateNew(String email)
        {
            var subscriber = new Subscriber() { Email= email };
            return subscriber;
        }

        protected Subscriber() { }

        public int Id { get; set; } 
        public String Email { get; set; }
    }
}
