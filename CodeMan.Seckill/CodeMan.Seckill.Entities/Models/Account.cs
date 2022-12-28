using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeMan.Seckill.Entities.Models
{
    [Table("account")]
    public class Account
    {
        [Column("user_id")]
        [Key]
        public int UserId { get; set; }
        [Column("username")]
        public string Username { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

    }
}