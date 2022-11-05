using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFCoreDB.DataLayer.Model
{
    [Table("audiientry", Schema = "blogging")] // 默认dbo变成blogging
    public class AuditEntry
    {
        [Key]
        public int AuditEntrtyId { get; set; }
        public string UserName { get; set; }
        public string Action { get; set; }
    }
}
