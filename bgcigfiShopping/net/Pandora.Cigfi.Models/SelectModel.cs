using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandora.Cigfi.Models
{
    [MessagePack.MessagePackObject(true)]
    public class SelectModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
