using System;
using System.Collections.Generic;
using System.Text;
using FXH.Redis.Extensions;


namespace Pandora.Cigfi.Common
{
    public enum RequestCheckType
    {
        LoginFailCount,
        Reg,
        SendSms,
        SendSmsIP,
        SmsVerifyFailIP,
        SmsVerify
    }
    public enum RedisKeyType
    {
        None,
        token,
        CmsSmsCode,
        RequestCheck
    }

    /// <summary>
    /// 输入验证
    /// </summary>
    public class CheckConfig
    {
        /// <summary>
        /// 输入统计时间
        /// </summary>
        public const int StatisticsMinute = 15;
        /// <summary>
        /// 最大次数
        /// </summary>
        public const int MaxCount = 5;
        /// <summary>
        /// 锁定N分钟
        /// </summary>
        public const int LockMinute = 10;
        /// <summary>
        /// 统计同一个ip
        /// </summary>
        public bool IsSameIP { get; set; }
    }

    /// <summary>
    /// 检测请求工具
    /// </summary>
    public class RequestCheckHelper
    {
        private IRedisCache CacheHelper;
        public RequestCheckHelper(IRedisCache redisCache)
        {
            CacheHelper = redisCache;
        }
        /// <summary>
        /// 检测请求次数
        /// </summary>
        public bool CheckRequest(RequestCheckType requestCheckType, string phoneNo, string ip)
        {
            //var CheckConfig = ServiceConfig.RequestCheck[requestCheckType.ToString()];
            StringBuilder rediskeysb = new StringBuilder($"{RedisKeyType.RequestCheck}:{requestCheckType}");
            /*if (CheckConfig.IsSameIP)
            {
                rediskeysb.Append($":{ip}");
            }*/
            rediskeysb.Append($":{phoneNo}");
            string rediskey = rediskeysb.ToString();

            bool result = false;
            var checkcache = CacheHelper.Get<Dictionary<string, int>>(rediskey);
            if (checkcache == null || !checkcache.ContainsKey("Count"))
            {
                result = true;
            }
            else if (checkcache["Count"] >= CheckConfig.MaxCount)
            {
                if (CheckConfig.StatisticsMinute != CheckConfig.LockMinute)
                {
                    CacheHelper.Add(rediskey, checkcache, TimeSpan.FromMinutes(CheckConfig.LockMinute));
                }
                result = false;
            }
            else
            {
                result = true;
            }
            return result;
        }
        /// <summary>
        /// 清除请求次数
        /// </summary>
        public void RemoveCheckRequest(RequestCheckType requestCheckType, string phoneNo, string ip)
        {
           // var CheckConfig = ServiceConfig.RequestCheck[requestCheckType.ToString()];
            StringBuilder rediskeysb = new StringBuilder($"{RedisKeyType.RequestCheck}:{requestCheckType}");
            /*if (CheckConfig.IsSameIP)
            {
                rediskeysb.Append($":{ip}");
            }*/
            rediskeysb.Append($":{phoneNo}");
            string rediskey = rediskeysb.ToString();

            CacheHelper.Remove(rediskey);
        }
        /// <summary>
        /// 更新请求次数
        /// </summary>
        public  void UpdateCheckRequest(RequestCheckType requestCheckType, string phoneNo, string ip)
        {

            //var CheckConfig = ServiceConfig.RequestCheck[requestCheckType.ToString()];
            StringBuilder rediskeysb = new StringBuilder($"{RedisKeyType.RequestCheck}:{requestCheckType}");
           /* if (CheckConfig.IsSameIP)
            {
                rediskeysb.Append($":{ip}");
            }*/
            rediskeysb.Append($":{phoneNo}");
            string rediskey = rediskeysb.ToString();
            var checkcache = CacheHelper.Get<Dictionary<string, int>>(rediskey);
            if (checkcache == null)
            {
                checkcache = new Dictionary<string, int>();
            }
            if (!checkcache.ContainsKey("Count"))
            {
                checkcache = new Dictionary<string, int>();
                checkcache["Count"] = 1;
            }
            else
            {
                checkcache["Count"] = checkcache["Count"] + 1;
            }
            CacheHelper.Add(rediskey, checkcache, TimeSpan.FromMinutes(CheckConfig.StatisticsMinute));
        }
    }

}
