using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pandora.Cigfi.Models.Cigfi.Rebate
{
    [Table("invite_rebate_ratio")]
    public class Rebate1
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int Level { get; set; }  // 1,2,3

        [Required]
        [MaxLength(50)]
        public string InviterLevelDesc { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal RebateRatio { get; set; }  // 百分比，比如30.00代表30%
    }
}
