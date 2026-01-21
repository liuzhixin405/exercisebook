namespace IdentityPaymentApi.Models;

public class Permission
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty; // e.g. payments.create
    public string? Description { get; set; }
}
