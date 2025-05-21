using System.Net;
using System.Text;
using System.IO;
using System;
using Utils;

namespace AliYunCore
{

	class HttpClient
	{

		public static string getResponse(Request request)
		{
			string postData = JSON.stringify(request.bizData);
			string contentMD5 = Encode.Base64Encode(Cryptography.MD5(postData));

			string url = request.protocol + "://" + request.domain + request.path;
			string query = request.getQueryString();

			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url + query);
			signatureHeader(httpWebRequest, request, contentMD5);

			using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
			{
				streamWriter.Write(postData);
				streamWriter.Flush();
			}

			HttpWebResponse httpResponse = null;
			try
			{
				httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				using (var streamReader = new StreamReader(httpResponse.GetResponseStream(), Encoding.UTF8))
				{
					string result = streamReader.ReadToEnd();
					return result;
				}
			}
			catch (WebException e)
			{
				httpResponse = (HttpWebResponse)e.Response;
				using (var streamReader = new StreamReader(httpResponse.GetResponseStream(), Encoding.UTF8))
				{
					string result = streamReader.ReadToEnd();
					return result;
				}
			}

		}

		private static void signatureHeader(HttpWebRequest httpWebRequest, Request request, string contentMD5)
		{
			string uuid = Guid.NewGuid().ToString();
			DateTime gmtTime = DateTime.Now;

			httpWebRequest.Method = request.httpMethod;
			httpWebRequest.Accept = "application/json";
			httpWebRequest.ContentType = "application/json";
			httpWebRequest.Date = DateTime.Now;

			WebHeaderCollection headers = httpWebRequest.Headers;

			headers.Add("Content-MD5", contentMD5);
			headers.Add("x-acs-version", request.version);
			headers.Add("x-acs-signature-nonce", uuid);
			headers.Add("x-acs-signature-version", request.signatureVersion);
			headers.Add("x-acs-signature-method", request.signatureMethod);

			StringBuilder str = new StringBuilder();
			str.Append("POST\n")
			   .Append("application/json\n")
			   .Append(contentMD5).Append("\n")
			   .Append("application/json\n")
			   .Append(gmtTime.ToUniversalTime().ToString("r")).Append("\n")
			   .Append("x-acs-signature-method:").Append(request.signatureMethod).Append("\n")
			   .Append("x-acs-signature-nonce:").Append(uuid).Append("\n")
			   .Append("x-acs-signature-version:").Append(request.signatureVersion).Append("\n")
			   .Append("x-acs-version:").Append(request.version).Append("\n")
			   .Append(request.path).Append(request.getQueryString());

			string signature = Encode.Base64Encode(Cryptography.HMACSHA1(str.ToString(), request.accessKeySecret));

			headers.Add("Authorization", "acs " + request.accessKeyId + ":" + signature);
		}


	}
}