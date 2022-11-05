using System.ComponentModel.DataAnnotations;

namespace EFCoreDB.DataLayer.Model
{
    public class AuditEntry
    {
        [Key]
        public int AuditEntrtyId { get; set; }
        public string UserName { get; set; }
        public string Action { get; set; }
    }
}
