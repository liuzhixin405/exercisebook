using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CAP.MySql.Model
{
    [Table("tBOrder")]
    public class Order
    {
        [Key]
        public int Id { get; set; }
        [Column("UserName")]
        public string Name { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
