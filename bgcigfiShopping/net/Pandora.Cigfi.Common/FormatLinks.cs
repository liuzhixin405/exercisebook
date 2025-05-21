using System.Text.RegularExpressions;

namespace Pandora.Cigfi.Common
{
    public static class FormatLinks
    {
        /// <summary>
        /// 获取前台其他地址格式化成数据库字段
        /// </summary>
        /// <param name="othersLink">其他地址</param>
        /// <returns></returns>
        public static string OthersLinkFormat(string othersLink)
        {
            string weibo = "";
            string twitter = "";
            string facebook = "";
            string telegram = "";
            string reddit = "";

            if (!string.IsNullOrEmpty(othersLink))
            {
                if (othersLink.Substring(othersLink.Length - 1, 1) != "\n")
                {
                    othersLink += "\n";
                }

                var weiboM = Regex.Match(othersLink, @"(\n)?(https://weibo.com.*?)(\n)");
                if (weiboM.Success)
                {
                    weibo = weiboM.Groups[2].Value;
                }

                var twitterM = Regex.Match(othersLink, @"(\n)?(https://twitter.com.*?)(\n)");
                if (twitterM.Success)
                {
                    twitter = twitterM.Groups[2].Value;
                }

                var facebookM = Regex.Match(othersLink, @"(\n)?(https://www.facebook.com.*?)(\n)");
                if (facebookM.Success)
                {
                    facebook = facebookM.Groups[2].Value;
                }

                var telegramM = Regex.Match(othersLink, @"(\n)?(https://t.me.*?)(\n)");
                if (telegramM.Success)
                {
                    telegram = telegramM.Groups[2].Value;
                }

                var redditM = Regex.Match(othersLink, @"(\n)?(https://www.reddit.com.*?)(\n)");
                if (redditM.Success)
                {
                    reddit = redditM.Groups[2].Value;
                }
            }


            string result = string.Format("\"weibo\":\"{0}\",\"twitter\":\"{1}\",\"facebook\":\"{2}\",\"telegram\":\"{3}\",\"reddit\":\"{4}\"", weibo, twitter, facebook, telegram, reddit);
            return "{" + result + "}";
        }

        /// <summary>
        /// 数据库其他地址格式化前台字符串
        /// </summary>
        /// <param name="othersLink"></param>
        /// <returns></returns>
        public static string OthersLinkUnFormat(string othersLink)
        {
            string result = "";
            if (string.IsNullOrEmpty(othersLink))
            {
                return "";
            }
            var weiboM = Regex.Match(othersLink, "\"weibo\":\"(.*?)\"");
            if (weiboM.Success)
            {
                if (weiboM.Groups[1].Value != "")
                {
                    result += weiboM.Groups[1].Value + "\n";
                }

            }

            var twitterM = Regex.Match(othersLink, "\"twitter\":\"(.*?)\"");
            if (twitterM.Success)
            {
                if (twitterM.Groups[1].Value != "")
                {
                    result += twitterM.Groups[1].Value + "\n";
                }

            }

            var facebookM = Regex.Match(othersLink, "\"facebook\":\"(.*?)\"");
            if (facebookM.Success)
            {
                if (facebookM.Groups[1].Value != "")
                {
                    result += facebookM.Groups[1].Value + "\n";
                }
            }

            var telegramM = Regex.Match(othersLink, "\"telegram\":\"(.*?)\"");
            if (telegramM.Success)
            {
                if (telegramM.Groups[1].Value != "")
                {
                    result += telegramM.Groups[1].Value + "\n";
                }

            }

            var redditM = Regex.Match(othersLink, "\"reddit\":\"(.*?)\"");
            if (redditM.Success)
            {
                if (redditM.Groups[1].Value != "")
                {
                    result += redditM.Groups[1].Value;
                }

            }

            return result;
        }
    }
}
