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
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace AuthenticationCenter.Realization
{
    public class JwtRsService : IJwtService
    {
        private readonly JwtTokenOptions _jwtTokenOptions;
        public JwtRsService(IOptionsMonitor<JwtTokenOptions> jwtTokenOptions)
        {
            _jwtTokenOptions = jwtTokenOptions.CurrentValue;
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

            string keyDir = Directory.GetCurrentDirectory();
            if(RsaHelper.TryGetKeyParameters(keyDir,true,out RSAParameters keyParams)== false)
            {
                keyParams = RsaHelper.GenerateAndSaveKey(keyDir);
            }

            var credentials = new SigningCredentials(new RsaSecurityKey(keyParams), SecurityAlgorithms.RsaSha256Signature);

            var token = new JwtSecurityToken(
                issuer:this._jwtTokenOptions.Issuer,
                audience:this._jwtTokenOptions.Audience,
                claims: claims,
                expires:DateTime.Now.AddMilliseconds(60),
                signingCredentials:credentials);
            var handler = new JwtSecurityTokenHandler();
           return handler.WriteToken(token);
        }
    }
}
