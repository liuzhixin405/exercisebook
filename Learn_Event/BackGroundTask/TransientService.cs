using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackGroundTask
{
    internal class TransientService
    {
        public Guid Id { get; }=Guid.NewGuid();
    }
}
