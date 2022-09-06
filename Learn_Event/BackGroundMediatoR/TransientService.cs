using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackGroundMediatR
{
    internal class TransientService
    {
        public Guid Id { get; }=Guid.NewGuid();
    }
}
