using System.Security.Claims;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authentication;
namespace MiniOpenApi
{
    public class Helper
    {
        public static string Serialize(ClaimsPrincipal cp)
        {
            AuthenticationTicket at = new AuthenticationTicket(cp,"cookie");
            return GetFormat().Protect(at);
        }
        public static AuthenticationTicket DeSerialize(string str)
        {
            return GetFormat().Unprotect(str);
        }
        private static SecureDataFormat<AuthenticationTicket> GetFormat()
        {
            var protector = DataProtectionProvider.Create("mini").CreateProtector("mini", "miniopenapi");
            return new SecureDataFormat<AuthenticationTicket>(TicketSerializer.Default, protector);
        }
    }
}
