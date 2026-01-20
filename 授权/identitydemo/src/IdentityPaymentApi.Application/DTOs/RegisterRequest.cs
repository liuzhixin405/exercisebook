using System.ComponentModel.DataAnnotations;

namespace IdentityPaymentApi.Application.DTOs;

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    public string? FullName { get; set; }
}
