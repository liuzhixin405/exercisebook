using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CAP.SqlServer.Model
{
    [Table("tBStock")]
    public class Stock
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int Num { get; set; }
    }
}
