using Microsoft.AspNetCore.Identity;

namespace IdentityPaymentApi.Models;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
}