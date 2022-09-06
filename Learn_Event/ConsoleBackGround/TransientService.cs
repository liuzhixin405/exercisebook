using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleBackGround
{
    internal class TransientService
    {
        public Guid Id { get; }=Guid.NewGuid();
    }
}
