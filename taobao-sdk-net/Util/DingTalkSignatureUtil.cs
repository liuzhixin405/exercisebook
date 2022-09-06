using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Top.Api.Util
{
    public class DingTalkSignatureUtil
    {
        private static string DEFAULT_ENCODING = "UTF-8";

        /* Signature method. */
        private static string ALGORITHM = "HmacSHA256";

        private static string NEW_LINE = "\n";

        // 获取签名所需要的字符串
        public static string GetCanonicalStringForIsv(long timestamp, string suiteTicket)
        {
            StringBuilder canonicalString = new StringBuilder();
            canonicalString.Append(timestamp);
            if (suiteTicket != null)
            {
                canonicalString.Append(NEW_LINE).Append(suiteTicket);
            }

            return canonicalString.ToString();
        }

        /**
         * 计算签名
         * @param canonicalString 签名
         * @param secret 签名秘钥
         * @return
         */
        public static string ComputeSignature(string secret, string canonicalString)
        {
            byte[] signData = Sign(Encoding.UTF8.GetBytes(canonicalString), Encoding.UTF8.GetBytes(secret));
            return Convert.ToBase64String(signData);
        }


        private static byte[] Sign(byte[] key, byte[] data)
        {
            HMACSHA256 sha256 = new HMACSHA256(data);
            return sha256.ComputeHash(key);
        }

        // 拼接url参数
        public static String ParamToQueryString(IDictionary<String, String> param, String charset)
        {

            if (param == null || param.Count == 0) {
                return null;
            }

            StringBuilder paramString = new StringBuilder();
            bool first = true;
            foreach (string key in param.Keys)
            {
                string value = param[key];
           

                if (!first)
                {
                    paramString.Append("&");
                }

                // Urlencode each request parameter
                paramString.Append(UrlEncode(key, charset));
                if (value != null)
                {
                    paramString.Append("=").Append(DingTalkSignatureUtil.UrlEncode(value, charset));
                }

                first = false;
            }

            return paramString.ToString();
        }

        /**
         * Encode a URL segment with special chars replaced.
         */
        public static String UrlEncode(String value, String encoding)
        {
            if (value == null)
            {
                return "";
            }


            String encoded = System.Web.HttpUtility.UrlEncode(value, System.Text.Encoding.UTF8);
            return encoded.Replace("+", "%20").Replace("*", "%2A")
                          .Replace("~", "%7E").Replace("/", "%2F");
        }

        /**
         * 生成随机数
         * @return
         */
        public static String GetRandomStr(int count)
        {
            String value = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                int number = random.Next(value.Length);
                sb.Append(CharAt(value, number));
            }
            return sb.ToString();
        }

        public static string CharAt(string value, int index)
        {
            if (index >= value.Length || index < 0)
            {
                return "";
            }
            return value.Substring(index, 1);
        }
    }
}
