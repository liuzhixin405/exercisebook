using System.Collections.Generic;
using System.Text;
using Model;

namespace AliYunCore{
    class DefaultProfile{
        public string regionId { set; get; }
        public string accessKeyId { set; get; }
        public string accessKeySecret { set; get; }

        public DefaultProfile(string regionId, string accessKeyId, string accessKeySecret){
            this.regionId = regionId;
            this.accessKeyId = accessKeyId;
            this.accessKeySecret = accessKeySecret;
        }
    }

    class Request{
        public string httpMethod { set; get; }
		public string protocol { set; get; }
		public string domain { set; get; }
		public string path { set; get; }
		public string version { set; get; }
		public string accessKeyId { set; get; }
		public string accessKeySecret { set; get; }
		public string signatureMethod { set; get; }
		public string signatureVersion { set; get; }
        public BizData bizData { set; get; }
        public Dictionary<string, string> queryMap { set; get; }

        public Request(BizData bizData, string path){
            this.httpMethod = "POST";
            this.protocol = "http";
            this.domain = "green.cn-shanghai.aliyuncs.com";
            this.version = "2017-01-12";
            this.signatureMethod = "HMAC-SHA1";
            this.signatureVersion = "1.0";
            this.queryMap = new Dictionary<string, string>();
            this.bizData = bizData;
            this.path = path;
        }

        public void addQueryParameter(string key, string value){
            this.queryMap.Add(key, value);
        }

        public string getQueryString(){
            StringBuilder sb = new StringBuilder("?");
            foreach(KeyValuePair<string, string> kv in this.queryMap){
                sb.Append(kv.Key).Append("=").Append(kv.Value).Append("&");
            }
            string str = sb.ToString();
            return str.Substring(0, str.Length - 1);
        }

    }
}