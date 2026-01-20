namespace IdentityPaymentApi.Models;

public class JwtSettings
{
    public string Issuer { get; set; } = "IdentityPaymentApi";
    public string Audience { get; set; } = "IdentityPaymentClients";
    public string Secret { get; set; } = "ChangeThisSecretToAStrongRandomValue123!";
    public int ExpirationMinutes { get; set; } = 60;
}