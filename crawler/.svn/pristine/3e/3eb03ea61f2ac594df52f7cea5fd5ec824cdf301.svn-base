using Crawler.Models.Attri;
using Crawler.Service.ExceptionManager;
using System;
using System.Collections.Generic;
using System.Text;

namespace Crawler.Models
{
    public class SeleCookie
    {
        [Custom]
        public string ig_did { get; set; }
        [Custom]
        public string mid { get; set; }
        [Custom]
        public string rur { get; set; }
        [Custom]
        public string csrftoken { get; set; }
        [Custom]
        public string ds_user_id { get; set; }
        [Custom]
        public string urlgen { get; set; }
        [Custom]
        public string sessionid { get; set; }


        public string this[string name]
        {
            get
            {
                switch (name)
                {

                    case "ig_did":
                        return ig_did;
                    case "mid":
                        return mid;
                    case "rur":
                        return rur;
                    case "csrftoken":
                        return csrftoken;
                    case "ds_user_id":
                        return ds_user_id;
                    case "urlgen":
                        return urlgen;
                    case "sessionid":
                        return sessionid;
                    default:
                        throw new CustomException($"未能获取-{name}-的值");

                }

            }
        }

    }
}
