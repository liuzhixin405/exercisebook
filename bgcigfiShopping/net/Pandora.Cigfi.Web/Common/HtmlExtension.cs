using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Pandora.Cigfi.Web.Common
{
    public static class HtmlExtension
    {/// <summary>
     /// 处理文章内容
     /// </summary>
     /// <param name="htmlCode"></param>
     /// <param name="url"></param>
     /// <returns></returns>
        public static string ConvertHtmlString(this byte[] htmlByteArray, string url)
        {
            if (url.Contains("people.com.cn"))
            {
                var GB2312 = System.Text.Encoding.GetEncoding("gb2312");
                var html = GB2312.GetString(htmlByteArray);
                return html;
            }
            return System.Text.Encoding.Default.GetString(htmlByteArray);
        }
        /// <summary>
        /// 处理文章内容
        /// </summary>
        /// <param name="htmlCode"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static HtmlDataModel ProcessHtml(this byte[] htmlByteArray, string url)
        {
            var htmlCode = ConvertHtmlString(htmlByteArray, url);
            var domainUrl = url.GetDomainUrl().ToLower();
            if (domainUrl.Contains("8btc"))
            {
                return htmlCode.Process8btc();
            }
            else if (domainUrl.Contains("odaily"))
            {
                return htmlCode.ProcessOdaily();
            }
            else if (domainUrl.Contains("xcong"))
            {
                return htmlCode.ProcessXcong();
            }
            else if (domainUrl.Contains("36kr.com"))
            {
                return htmlCode.Process36kr();
            }
            else if (domainUrl.Contains("cointelegraph.cn.com"))
            {
                return htmlCode.ProcessCointelegraph();
            }
            else if (domainUrl.Contains("tech.ifeng.com"))
            {
                return htmlCode.ProcessIfeng();
            }
            else if (domainUrl.Contains("tech.ifeng.com"))
            {
                return htmlCode.ProcessIfeng();
            }
            else if (domainUrl.Contains("people.com.cn"))
            {
                return htmlCode.ProcessPeople();
            }
            else
            {
                var result = new HtmlDataModel();
                if (htmlCode.Contains(@"<section class=""single-main-container"">") && htmlCode.Contains("okex", StringComparison.CurrentCultureIgnoreCase))
                {
                    //okex教程
                    result.content = Regex.Match(htmlCode, @"<p>(?<content>[\s\S]*?)</p>\s+<div").Groups["content"].Value;
                    result.title = Regex.Match(htmlCode, @"<title>(?<title>.{1,100}?)</title>").Groups[1].Value.Trim();
                }
                else if (htmlCode.Contains(@"<div class=""articleContent"">") && htmlCode.Contains("binance", StringComparison.CurrentCultureIgnoreCase))
                {
                    //币安教程
                    var resMatch = Regex.Match(htmlCode, @"<div class=""articleContent""><h1>(?<title>.{1,200}?)</h1>(.{1,3000}?)<div class=""tooltip"">(.{1,200}?)</div></div></div></div>(?<content>[\s\S]*?)<div class=""relatedArticles"">");
                    result.content = resMatch.Groups["content"].Value;
                    result.title = resMatch.Groups["title"].Value;
                }
                else if (htmlCode.Contains(@"<div class=""course-wrap""") && htmlCode.Contains("huobi", StringComparison.CurrentCultureIgnoreCase))
                {
                    //火币教程
                    var resMatch = Regex.Match(htmlCode, @"<div class=(""|')course-wrap(""|')(.{0,100}?)>(?<content>[\s\S]*?)<div class=(""|')novice-info(""|')(.{0,100}?)>");
                    result.content = resMatch.Groups["content"].Value;
                    var titleRes = Regex.Match(result.content, @"<p class=(""|')title(""|')(.{0,50}?)>(?<title>.{1,200}?)</p>");
                    result.content = result.content.Replace(titleRes.Value, string.Empty);
                    result.title = titleRes.Groups["title"].Value;
                }
                else
                {
                    result.content = Regex.Match(htmlCode, @"<div>[\s\S]*?</body>").ToString();
                    var titleStr = Regex.Match(htmlCode, @"(?<=<title.*>)(?<title>.{1,300}?)(?=</title>)").ToString();
                    result.title = titleStr.Contains("_") ? titleStr.Substring(0, titleStr.IndexOf("_")) : titleStr;
                } 
                result.keywords = Regex.Match(htmlCode, @"(?<=name=""keywords"" content="")(?<keywords>.{1,300}?)(?="")").ToString();
                result.desc = Regex.Match(htmlCode, @"(?<=name=""description"" content="")(?<description>.{1,500}?)(?="")").ToString();
                return result;
            }
            return null;
        }
        //
        /// <summary>
        /// 处理 cointelegraph.cn.com  https://cointelegraph.cn.com/news/telegrams-ton-os-to-go-open-source-on-github-tomorrow
        /// </summary>
        /// <param name="htmlCode"></param>
        /// <returns></returns>
        public static HtmlDataModel ProcessCointelegraph(this string htmlCode)
        {
            var result = new HtmlDataModel();
            var html = htmlCode.LoadHtml();
            result.title = html.DocumentNode.SelectSingleNode("//h1[@class='post__title']")?.InnerHtml;
            var contentnode = html.DocumentNode.SelectSingleNode("//div[@class='post post-page__article post_asia']");//.SelectSingleNode("//div[@class='post__content-wrapper']");
            if (contentnode != null)
            { 
                contentnode.SelectSingleNode("//article/div[@data-v-13771312='']")?.Remove();
                contentnode.SelectSingleNode("//article/div[@data-v-00f872dd='']")?.Remove();
                var source = contentnode.SelectNodes("//source");
                if (source != null)
                {
                    foreach (var item in source)
                    {
                        item.Remove();
                    }
                }
                contentnode.SelectSingleNode("//article/h1")?.Remove();
                var tagnode = contentnode.SelectSingleNode("//div[@class='tags-list post__block post__block_tags']");
                if (tagnode != null)
                {
                    tagnode.Remove();
                    //contentnode.RemoveChild(tagnode);
                }
                var relatedlist = contentnode.SelectSingleNode("//div[@class='related-list']");
                if (relatedlist != null)
                {
                    relatedlist.Remove();
                    //contentnode.RemoveChild(relatedlist);
                }
               result.content = contentnode?.InnerHtml;

            }

            return html.GetKeywordsAndDesc(result);
        }
        //
        /// <summary>
        /// 处理 36氪  https://36kr.com/p/695545371662725
        /// </summary>
        /// <param name="htmlCode"></param>
        /// <returns></returns>
        public static HtmlDataModel Process36kr(this string htmlCode)
        {
            var result = new HtmlDataModel();
            var html = htmlCode.LoadHtml();
            result.title = html.DocumentNode.SelectSingleNode("//h1[@class='article-title margin-bottom-20 common-width']")?.InnerHtml;
            result.content = html.DocumentNode.SelectSingleNode("//div[@class='common-width content articleDetailContent kr-rich-text-wrapper']")?.InnerHtml;

            return html.GetKeywordsAndDesc(result);
        }
        
        /// <summary>
        /// 处理  https://tech.ifeng.com/c/7wFo7sBHccC
        /// </summary>
        /// <param name="htmlCode"></param>
        /// <returns></returns>
        public static HtmlDataModel ProcessIfeng(this string htmlCode)
        {
            var result = new HtmlDataModel();
            var html = htmlCode.LoadHtml();
            result.title = html.DocumentNode.SelectSingleNode("//h1[@class='topic-3bY8Hw-9']")?.InnerHtml;
            result.content = html.DocumentNode.SelectSingleNode("//div[@class='main_content-LcrEruCc']")?.InnerHtml;

            return html.GetKeywordsAndDesc(result);
        }

        /// <summary>
        /// 处理  http://blockchain.people.com.cn/n1/2020/0430/c417685-31694555.html
        /// </summary>
        /// <param name="htmlCode"></param>
        /// <returns></returns>
        public static HtmlDataModel ProcessPeople(this string htmlCode)
        {
            var result = new HtmlDataModel();
            var html = htmlCode.LoadHtml();
            var title = html.DocumentNode.SelectSingleNode("//div[@class='clearfix w1000_320 text_title']//h1")?.InnerHtml ?? "";
            var content= html.DocumentNode.SelectSingleNode("//div[@id='rwb_zw']")?.InnerHtml??"";
            
            result.title = title;
            result.content = content;

            return html.GetKeywordsAndDesc(result);
        }
        //
        /// <summary>
        /// 处理 小葱  xcong.com
        /// </summary>
        /// <param name="htmlCode"></param>
        /// <returns></returns>
        public static HtmlDataModel ProcessXcong(this string htmlCode)
        {
            var result = new HtmlDataModel();
            var html = htmlCode.LoadHtml();
            result.title = html.DocumentNode.SelectSingleNode("//h1[@class='article-title']")?.InnerHtml;
            result.content = html.DocumentNode.SelectSingleNode("//div[@class='article-content']")?.InnerHtml;

            return html.GetKeywordsAndDesc(result);
        }

        /// <summary>
        /// 处理 星球日报 odaily.com
        /// </summary>
        /// <param name="htmlCode"></param>
        /// <returns></returns>
        public static HtmlDataModel ProcessOdaily(this string htmlCode)
        {
            var result = new HtmlDataModel();
            var html = htmlCode.LoadHtml();
            result.title = html.DocumentNode.SelectSingleNode("//div[@class='_30dHyM_c']//h1")?.InnerHtml;
            result.content = html.DocumentNode.SelectSingleNode("//div[@class='_3739r7Mk']")?.InnerHtml;

            return html.GetKeywordsAndDesc(result);
        }

        /// <summary>
        /// 处理 巴比特 8btc.com
        /// </summary>
        /// <param name="htmlCode"></param>
        /// <returns></returns>
        public static HtmlDataModel Process8btc(this string htmlCode)
        {
            var result = new HtmlDataModel();
            var html = htmlCode.LoadHtml();
            result.title = html.DocumentNode.SelectSingleNode("//div[@class='bbt-container']//h1")?.InnerHtml;
            var rows = html.DocumentNode.SelectNodes("//div[@class='bbt-html']");
            if (rows != null && rows.Count > 0)
                foreach (var r in rows)
                {
                    result.content += r.InnerHtml;
                }
            return html.GetKeywordsAndDesc(result);
        }

        /// <summary>
        /// 获取关键词和描述
        /// </summary>
        /// <param name="html"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static HtmlDataModel GetKeywordsAndDesc(this HtmlDocument html, HtmlDataModel result)
        {
            result.keywords = html.GetMetaValue("keywords");
            result.desc = html.GetMetaValue("description");
            return result;
        }

        /// <summary>
        /// 获取关键词和描述
        /// </summary>
        /// <param name="html"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static string GetMetaValue(this HtmlDocument html, string metaName)
        {
            var r = html.DocumentNode.SelectSingleNode($"//meta[@name='{metaName}']")?.GetAttributeValue("content", "");
            return r== "undefined"?"":r;
        }

        /// <summary>
        /// 获取域名
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetDomainUrl(this string url)
        {
            url = url.Replace("https://", "").Replace("http://", "");
            url = url.Substring(0, url.IndexOf('/'));
            return url;
        }

        /// <summary>
        /// 加载内容
        /// </summary>
        /// <param name="htmlCode"></param>
        /// <returns></returns>
        private static HtmlDocument LoadHtml(this string htmlCode)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlCode);
            return htmlDoc;
        }
    }

    public class HtmlDataModel
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 正文
        /// </summary>
        public string content { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string desc { get; set; }
        /// <summary>
        /// 关键词
        /// </summary>
        public string keywords { get; set; }
    }
}
