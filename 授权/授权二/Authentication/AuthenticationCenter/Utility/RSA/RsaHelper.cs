using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AuthenticationCenter.Utility.RSA
{
    public class RsaHelper
    {
        public static bool TryGetKeyParameters(string filePath,bool withPrivate,out RSAParameters keyParameters)
        {
            string filename = withPrivate ? "key.json" : "key.public.json";
            string fileTotalPath = Path.Combine(filePath, filename);
            keyParameters = default(RSAParameters);
            if (!File.Exists(fileTotalPath))
            {
                return false;
            }
            else
            {
                keyParameters = JsonConvert.DeserializeObject<RSAParameters>(File.ReadAllText(fileTotalPath));
                return true;
            }
        }

        public static RSAParameters GenerateAndSaveKey(string filePath,bool withPrivate = true)
        {
            RSAParameters publicKeys, privateKeys;
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                try
                {
                    privateKeys = rsa.ExportParameters(true);
                    publicKeys = rsa.ExportParameters(false);
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }

                File.WriteAllText(Path.Combine(filePath, "key.json"), JsonConvert.SerializeObject(privateKeys));
                File.WriteAllText(Path.Combine(filePath, "key.public.json"), JsonConvert.SerializeObject(publicKeys));
                return withPrivate ? privateKeys : publicKeys;
            }
        }
    }
}
