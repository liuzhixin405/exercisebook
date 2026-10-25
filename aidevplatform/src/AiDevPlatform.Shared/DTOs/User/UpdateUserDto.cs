namespace AiDevPlatform.Shared.DTOs.User;

public class UpdateUserDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Avatar { get; set; }
    public string? TimeZone { get; set; }
    public string? Language { get; set; }
}
