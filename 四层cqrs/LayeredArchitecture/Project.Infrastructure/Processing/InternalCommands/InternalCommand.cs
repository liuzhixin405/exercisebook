using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.Processing.InternalCommands
{
    public class InternalCommand
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string Data { get; set; }
        public DateTime? ProcessedDate { get; set; }
    }
}
