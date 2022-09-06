using AuthenticationCenter.Interface;
using AuthenticationCenter.Models;
using AuthenticationCenter.Utility.RSA;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationCenter.Realization
{
    public class JwtHsService : IJwtService
    {
        private readonly JwtTokenOptions _jwtServiceOptions;
        public JwtHsService(IOptionsMonitor<JwtTokenOptions> jwtTokenOptions)
        {
            this._jwtServiceOptions = jwtTokenOptions.CurrentValue;
        }
        public string GetToken(CurrentUserModel userModel)
        {
           
            var claims = new[]
            {
                   new Claim(ClaimTypes.Name,userModel.Name),
                   new Claim("EMail", userModel.EMail),
                   new Claim("Account", userModel.Account),
                   new Claim("Age", userModel.Age.ToString()),
                   new Claim("Id", userModel.Id.ToString()),
                   new Claim("Mobile", userModel.Mobile),
                   new Claim(ClaimTypes.Role,userModel.Role),
                   new Claim("Sex", userModel.Sex.ToString())//各种信息拼装
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._jwtServiceOptions.SecurityKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: this._jwtServiceOptions.Issuer,
                audience: this._jwtServiceOptions.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),//5分钟有效期
                notBefore: DateTime.Now.AddMinutes(1),//1分钟后有效
                signingCredentials: creds);
            string returnToken = new JwtSecurityTokenHandler().WriteToken(token);
            return returnToken;
        }
    }
}
