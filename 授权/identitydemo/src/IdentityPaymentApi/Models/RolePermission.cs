namespace IdentityPaymentApi.Models;

public class RolePermission
{
    public int Id { get; set; }
    public string RoleId { get; set; } = string.Empty; // maps to AspNetRoles.Id
    public int PermissionId { get; set; }
}
