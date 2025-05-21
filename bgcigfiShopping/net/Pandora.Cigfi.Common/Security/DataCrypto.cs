using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using FXH.Common.Logger;

namespace Pandora.Cigfi.Common.Security
{
    /// <summary>
    /// 数据加密、解密类
    /// </summary>
    public static class DataCrypto
    {
        /// <summary>
        /// 对输入的Base64字符串进行解码
        /// </summary>
        /// <param name="input">输入的Base64字符串</param>
        /// <returns>解码后的字符串</returns>
        public static string Base64StringDecode(string input)
        {
            byte[] decbuff = Convert.FromBase64String(input);
            return System.Text.Encoding.UTF8.GetString(decbuff);
        }

        /// <summary>
        /// 对输入字符串进行Base64编码
        /// </summary>
        /// <param name="input">输入的字符串</param>
        /// <returns>Base64编码后的字符串</returns>
        public static string Base64StringEncode(string input)
        {
            byte[] encbuff = System.Text.Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(encbuff);
        }

        /// <summary>
        /// 对输入字符串进行MD5加密，返回小写形式的加密字符串，字符串为32字符的十六进制格式
        /// </summary>
        /// <param name="input">待加密的字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string MD5(string input)
        {
            if (null != input)
            {
                using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
                {
                    byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                    return BitConverter.ToString(data).Replace("-", string.Empty).ToLower(CultureInfo.CurrentCulture);
                }
            }
            else
            {
                return "empty";
            }
        }

        /// <summary>
        /// 对输入字符串进行GB2312格式的MD5加密，返回小写形式的加密字符串，字符串为32字符的十六进制格式
        /// 此方法主要用于整合其他系统，兼容其他ASP系统的加密算法（ASP新MD5算法），与SiteWeaver的算法不完全兼容（ASP旧MD5算法）
        /// </summary>
        /// <param name="input">待加密的字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string MD5GB2312(string input)
        {
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] data = md5.ComputeHash(Encoding.GetEncoding("gb2312").GetBytes(input));
                return BitConverter.ToString(data).Replace("-", string.Empty).ToLower(CultureInfo.CurrentCulture);
            }
        }

        /// <summary>
        /// 对输入字符串进行SHA1加密，返回小写形式的加密字符串，字符串为40字符的十六进制格式
        /// </summary>
        /// <param name="input">待加密的字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string Sha1(string input)
        {
            using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
            {
                byte[] data = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(data).Replace("-", string.Empty).ToLower(CultureInfo.CurrentCulture);
            }
        }

        /// <summary>
        /// 旧版中密码哈希值保存为16位，在新版中采用32位保存
        /// 将需验证的密码哈希值分别与密码明文MD5加密后的32位字符串以及密码MD5加密后从8位开始取16位的字符串进行比较
        /// 两个条件满足其一，则验证通过
        /// </summary>
        /// <param name="hashValue">需要对比的密码哈希值</param>
        /// <param name="plaintext">密码明文</param>
        /// <returns>如果验证正确，则为 true；否则为 false。</returns>
        public static bool ValidateMD5(string hashValue, string plaintext)
        {
            string encryptedValue = MD5(plaintext);
            return (string.Compare(hashValue, encryptedValue, StringComparison.Ordinal) == 0) || (string.Compare(hashValue, encryptedValue.Substring(8, 16), StringComparison.Ordinal) == 0);
        }

        /// <summary>
        /// 8位密钥加密
        /// </summary>
        /// <param name="encryptValue"></param>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public static string EncryptString(string encryptValue, string sKey)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(encryptValue);
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                //以下两句限定了输入密钥只能为英文文本
                //des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
                //des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
                //注意几种字符编码的区别：ASCII编码对字符串中每个字符使用8位即一个byte来表示，所以这种方式无法表示全角
                //字符或者汉字（一个全角字符或汉字需要16位即两个byte来表示）。字符串里的每个全角字符或者汉字都看成一个字符
                //Unicode编码对字符串中每个字符使用16位即两个byte来表示，即使是一个半角字符也用16位来表示（虽然一个半角
                //字符用8位表示就足够了）。
                //Default是指根据需要当遇到一个半角字符时用8位表示，遇到一个全角或者汉字时用16位来表示
                //以下两句可以使输入密钥为中文或英文文本
                des.Key = System.Text.Encoding.Default.GetBytes(sKey);
                des.IV = System.Text.Encoding.Default.GetBytes(sKey);
                ICryptoTransform desencrypt = des.CreateEncryptor();
                byte[] result = desencrypt.TransformFinalBlock(data, 0, data.Length);//进行加密
                return BitConverter.ToString(result);
            }
            catch (Exception ex)
            {
                LogExtension.LogError("加密出错，原因：" + ex.Message);
                return "";
            }
        }

        /// <summary>
        /// 8位密钥3des解密
        /// </summary>
        /// <param name="encryptedValue"></param>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public static string DecryptString(string encryptedValue, string sKey)
        {
            try
            {
                string[] sInput = encryptedValue.Split("-".ToCharArray());
                byte[] data = new byte[sInput.Length];
                for (int i = 0; i < sInput.Length; i++)
                {
                    data[i] = byte.Parse(sInput[i], NumberStyles.HexNumber);
                }
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                //以下两句限定了输入密钥只能为英文文本
                //des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
                //des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
                //以下两句可以使输入密钥为中文或英文文本
                des.Key = System.Text.Encoding.Default.GetBytes(sKey);
                des.IV = System.Text.Encoding.Default.GetBytes(sKey);
                ICryptoTransform desencrypt = des.CreateDecryptor();
                byte[] result = desencrypt.TransformFinalBlock(data, 0, data.Length);//进行解密
                return Encoding.UTF8.GetString(result);
            }
            catch (Exception ex)
            {
                LogExtension.LogError("解密出错，原因：" + ex.Message);

                return "";
            }
        }
    }

}
