namespace IdentityPaymentApi.Models;

public class UserPermission
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty; // maps to AspNetUsers.Id
    public int PermissionId { get; set; }
    public bool IsAllowed { get; set; } = true;
}
