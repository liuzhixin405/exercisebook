using System;
using System.IO;
using System.Security.Cryptography;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Utils
{
	public static class Encode
	{
		public static string Base64Encode(byte[] src)
		{
			return Convert.ToBase64String(src, 0, src.Length);
		}

		public static string Base64Encode(string src)
		{
			byte[] bytedata = Encoding.UTF8.GetBytes(src);
			return Convert.ToBase64String(bytedata, 0, bytedata.Length);
		}

	}

	public static class Cryptography
	{
		public static byte[] HMACSHA1(string src, string key)
		{
			byte[] keyByte = Encoding.UTF8.GetBytes(key);
			HMACSHA1 hmacsha1 = new HMACSHA1(keyByte);

			byte[] messageByte = Encoding.UTF8.GetBytes(src);
			return hmacsha1.ComputeHash(messageByte);
		}

		public static byte[] MD5(string src)
		{
			MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
			byte[] bytValue, bytHash;
			bytValue = Encoding.UTF8.GetBytes(src);
			return bytHash = md5.ComputeHash(bytValue);
		}


	}


	public static class JSON
	{
		public static T parse<T>(string jsonString)
		{
			using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
			{
				return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(ms);
			}
		}

		public static string stringify<T>(T jsonObject)
		{
			using (var ms = new MemoryStream())
			{
				new DataContractJsonSerializer(typeof(T)).WriteObject(ms, jsonObject);
				return Encoding.UTF8.GetString(ms.ToArray());
			}
		}

	}

}