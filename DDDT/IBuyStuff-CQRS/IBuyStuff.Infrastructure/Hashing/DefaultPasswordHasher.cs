using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XSystem.Security.Cryptography;

namespace IBuyStuff.Infrastructure.Hashing
{
    public class DefaultPasswordHasher : IHashingService
    {
        public string Hash(string clearPassword)
        {
            return HashInternal(clearPassword);
        }

        public bool Validate(string clearPassword, string hash)
        {
            return HashInternal(clearPassword) == hash;
        }
        private string HashInternal(string password)
        {
            const string defaultSalt = "112358";

            var md5 = new MD5CryptoServiceProvider();
            var digest = md5.ComputeHash(Encoding.UTF8.GetBytes(string.Concat(password, defaultSalt)));
            var base64Digest = Convert.ToBase64String(digest, 0, digest.Length);
            return base64Digest.Substring(0, base64Digest.Length - 2);
        }
    }
}
