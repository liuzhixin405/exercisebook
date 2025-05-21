using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Net;
using System.IO;
using Microsoft.Extensions.Configuration;
using FXH.Web.Extensions.Http;
using System.Collections;
using System.Threading.Tasks;
using System.Net.Http;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Formula.Functions;
using FXH.Common.Data;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Drawing;
using System.Drawing.Printing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Pandora.Cigfi.Common.Images;

namespace Pandora.Cigfi.Common
{
    /// <summary>
    /// 常用函数帮助类
    /// </summary>
    public static class CommonUtils
    {

        public static IConfiguration Configuration { get; set; }

        #region 静态变量
        /// <summary>
        /// 得到正则编译参数设置
        /// </summary>
        /// <returns>参数设置</returns>
        public static RegexOptions GetRegexCompiledOptions()
        {
            return RegexOptions.None;
        }


        public static string PrefixKey
        {
            get
            {
                string strPrefixKey = Configuration["DataCenter:SystemSetting:CMSPrefixKey"];// ConfigurationManager.AppSettings["Pandora.CMSPrefixKey"];
                if (string.IsNullOrEmpty(strPrefixKey))
                {
                    strPrefixKey = "comcmsntc_";
                }
                return strPrefixKey;
            }
        }

        /// <summary>
        /// 与小程序验签的盐值
        /// </summary>
        public static readonly string SIGNSALT = "comcms";// ConfigurationManager.AppSettings["SignSalt"];


        /// <summary>
        /// 订单状态 "未确认", "已确认", "已完成", "已取消" 
        /// </summary>
        public static string[] OrdersState = { "未确认", "已确认", "已完成", "已取消" };
        /// <summary>
        /// 支付状态 { "未支付", "已支付", "已退款" }
        /// </summary>
        public static string[] PaymentState = { "未支付", "已支付", "已退款" };
        /// <summary>
        /// 配送状态 "未配送", "配货中", "已配送", "已收到", "退货中", "已退货"
        /// </summary>
        public static string[] DeliverState = { "未配送", "配货中", "已配送", "已收到", "退货中", "已退货" };
        #endregion

        #region 验证部分
        /// <summary>
        /// 验证是否为正整数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsInt(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return false;
            //return Regex.IsMatch(str, @"^[0-9]*$");
            return Regex.IsMatch(str, @"^(-?)(\d+)$");
        }
        /// <summary>
        /// 验证是否为字母或者数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsWords(string str)
        {
            return Regex.IsMatch(str, @"^[A-Za-z0-9]+$");
        }

        /// <summary>
        /// 验证对象是否为货币格式
        /// </summary>
        /// <param name="obj">验证对象</param>
        /// <returns>货币格式返回true，否则返回false</returns>
        public static bool IsDecimal(object obj)
        {
            bool flag = true;
            if (obj.GetType() != decimal.Parse("10.11").GetType())
            {
                try
                {
                    decimal.Parse((string)obj);
                }
                catch
                {
                    flag = false;
                }
            }
            return flag;
        }

        /// <summary>
        /// 检测是否是正确的Url
        /// </summary>
        /// <param name="strUrl">要验证的Url</param>
        /// <returns>判断结果</returns>
        public static bool IsURL(string strUrl)
        {
            return Regex.IsMatch(strUrl, @"^(http|https)\://([a-zA-Z0-9\.\-]+(\:[a-zA-Z0-9\.&%\$\-]+)*@)*((25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])|localhost|([a-zA-Z0-9\-]+\.)*[a-zA-Z0-9\-]+\.(com|edu|gov|int|mil|net|org|biz|arpa|info|name|pro|aero|coop|museum|[a-zA-Z]{1,10}))(\:[0-9]+)*(/($|[a-zA-Z0-9\.\,\?\'\\\+&%\$#\=~_\-]+))*$");
        }

        /// <summary>
        /// 检测是否是正确的邮箱
        /// </summary>
        /// <param name="strUrl">要验证的Url</param>
        /// <returns>判断结果</returns>
        public static bool IsEmail(string strUrl)
        {
            return Regex.IsMatch(strUrl, @"^([\w-\.]+)@([\w-\.]+)(\.[a-zA-Z0-9]+)$");
        }
        /// <summary>
        /// 获取当前网站根目录网址 不带 /
        /// </summary>
        /// <returns></returns>
        public static string GetServerUrl()
        {
            string http = "http://";
            if (HttpContextHelper.Current.Request.IsHttps) //判断是否是https
            {
                http = "https://";
            }
            string port = HttpContextHelper.Current.Request.Host.Port.ToString();
            if (string.IsNullOrEmpty(port) || port == "80" || port == "443")
                return http + HttpContextHelper.Current.Request.Host.Host;
            else
                return http + HttpContextHelper.Current.Request.Host.Host + ":" + port;

        }

        /// <summary>
        /// 判断是否为base64字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsBase64String(string str)
        {
            //A-Z, a-z, 0-9, +, /, =
            return Regex.IsMatch(str, @"[A-Za-z0-9\+\/\=]");
        }
        /// <summary>
        /// 检测是否有Sql危险字符
        /// </summary>
        /// <param name="str">要判断字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsSafeSqlString(string str)
        {
            return !Regex.IsMatch(str, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
        }
        /// <summary>
        /// 验证是否为手机号码
        /// </summary>
        /// <param name="str">手机号码</param>
        /// <returns></returns>
        public static bool IsTel(string str)
        {
            if (str.Length != 11)
                return false;
            return Regex.IsMatch(str, @"^1[3|4|5|7|8|9][0-9]\d{4,8}$");
        }
        /// <summary>
        /// 是否为ip
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIP(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }
        /// <summary>
        /// 检测是否符合email格式
        /// </summary>
        /// <param name="strEmail">要判断的email字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsValidEmail(string strEmail)
        {
            return Regex.IsMatch(strEmail, @"^[\w\.]+([-]\w+)*@[A-Za-z0-9-_]+[\.][A-Za-z0-9-_]");
        }

        public static bool IsIPSect(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){2}((2[0-4]\d|25[0-5]|[01]?\d\d?|\*)\.)(2[0-4]\d|25[0-5]|[01]?\d\d?|\*)$");
        }
        /// <summary>
        /// 返回指定IP是否在指定的IP数组所限定的范围内, IP数组内的IP地址可以使用*表示该IP段任意, 例如192.168.1.*
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="iparray"></param>
        /// <returns></returns>
        public static bool InIPArray(string ip, string[] iparray)
        {
            string[] userip = CommonUtils.SplitString(ip, @".");

            for (int ipIndex = 0; ipIndex < iparray.Length; ipIndex++)
            {
                string[] tmpip = CommonUtils.SplitString(iparray[ipIndex], @".");
                int r = 0;
                for (int i = 0; i < tmpip.Length; i++)
                {
                    if (tmpip[i] == "*")
                        return true;

                    if (userip.Length > i)
                    {
                        if (tmpip[i] == userip[i])
                            r++;
                        else
                            break;
                    }
                    else
                        break;
                }

                if (r == 4)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 分割字符串
        /// </summary>
        public static string[] SplitString(string strContent, string strSplit)
        {
            if (!string.IsNullOrWhiteSpace(strContent))
            {
                if (strContent.IndexOf(strSplit) < 0)
                    return new string[] { strContent };

                return Regex.Split(strContent, Regex.Escape(strSplit), RegexOptions.IgnoreCase);
            }
            else
                return new string[0] { };
        }
        /// <summary>
        /// 判断元素是否在列表中
        /// </summary>
        /// <param name="strin">判断字符</param>
        /// <param name="arraystring">使用,号隔开</param>
        /// <returns></returns>
        public static bool IsInArrayString(string strin, string arraystring)
        {
            bool flag = false;
            if (!string.IsNullOrEmpty(arraystring))
            {
                string[] srrids = arraystring.Split(new string[] { "," }, StringSplitOptions.None);
                List<string> kids = new List<string>();
                foreach (string s in srrids)
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        kids.Add(s.Trim());//加入允许使用的商品列表
                    }
                }
                if (kids.FindIndex(s => s == strin) >= 0)
                    flag = true;
            }
            return flag;
        }
        /// <summary>
        /// 验证身份证号码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool CheckCardId(string str)
        {
            if (string.IsNullOrWhiteSpace(str) || str.Length != 18)
                return false;
            string number17 = str.Substring(0, 17);
            string number18 = str.Substring(17);
            string check = "10X98765432";
            int[] num = { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };
            int sum = 0;
            for (int i = 0; i < number17.Length; i++)
            {
                sum += Convert.ToInt32(number17[i].ToString()) * num[i];
            }
            sum %= 11;
            if (number18.Equals(check[sum].ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 获取部分
        /// <summary>
        /// 上传文件保存名字-随机数
        /// </summary>
        /// <returns></returns>
        public static string CusImageName(string base64Image)
        {
            string saveName = MD5(DateTime.Now.ToString("yyyyMMdd")) + GetRND(1000, 9999).ToString();
            var result= saveName + ImageHelper.GetFileSuffix(base64Image);
            result = Regex.Replace(result, "[^0-9a-zA-Z.]", "_");
            return result;
        }

        /// <summary>
        /// 上传文件保存名字-MD5
        /// </summary>
        /// <returns></returns>
        public static string CusMD5Name(string name, string base64Image)
        {
            return MD5(name) + ImageHelper.GetFileSuffix(base64Image);
        }
        /// <summary>
        /// 返回字符串真实长度, 1个汉字长度为2
        /// </summary>
        /// <returns>字符长度</returns>
        public static int GetStringLength(string str)
        {
            return Encoding.UTF8.GetBytes(str).Length;
        }
        /// <summary>
        /// MD5函数
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <returns>MD5结果</returns>
        public static string MD5(string str)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.ASCII.GetBytes(str));
                var strResult = BitConverter.ToString(result);
                return strResult.Replace("-", "");
            }
        }
        /// <summary>
        /// SHA256函数
        /// </summary>
        /// /// <param name="str">原始字符串</param>
        /// <returns>SHA256结果</returns>
        public static string SHA256(string str)
        {
            using(var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var result = sha256.ComputeHash(Encoding.ASCII.GetBytes(str));
                var strResult = BitConverter.ToString(result);
                return strResult.Replace("-", "");
            }
        }

        /// <summary>
        /// 获取指定文件的扩展名
        /// </summary>
        /// <param name="fileName">指定文件名</param>
        /// <returns>扩展名(.xxx)</returns>
        public static string GetFileExtName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName) || fileName.IndexOf('.') <= 0)
                return "";

            fileName = fileName.ToLower().Trim();

            return fileName.Substring(fileName.LastIndexOf('.'), fileName.Length - fileName.LastIndexOf('.'));
        }

        private static Random rd = new Random();
        /// <summary>
        /// 获取随机整数
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns></returns>
        public static int GetRND(int min, int max)
        {
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            return rnd.Next(min, max);
        }
        /// <summary>
        /// 生成上传文件保存名字
        /// </summary>
        /// <returns>不带扩展名名字</returns>
        public static string CreateUploadSaveName()
        {
            string saveName = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + GetRND(1000, 9999).ToString();
            return saveName;
        }
        /// <summary>
        /// 生成订单号
        /// </summary>
        /// <returns>订单号</returns>
        public static string GetOrderNum()
        {
            string ordernum = DateTime.Now.ToString("yyyyMMddHHmm") + GetRND(1000, 9999).ToString();
            return ordernum;
        }
        private static char[] constant = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        /// <summary>
        /// 获取随机字符串
        /// </summary>
        /// <param name="strLength">字符长度</param>
        /// <returns>返回随机字符串</returns>
        public static string GetRandomChar(int strLength)
        {
            StringBuilder newRandom = new StringBuilder(62);
            Random rd = new Random(Guid.NewGuid().GetHashCode());
            for (int i = 0; i < strLength; i++)
            {
                newRandom.Append(constant[rd.Next(62)]);
            }
            return newRandom.ToString();
        }
        /// <summary>
        /// 获取当前附近坐标点
        /// </summary>
        /// <param name="longitude">经度</param>
        /// <param name="latitude">纬度</param>
        /// <param name="aboutRadius">附近半径，千米，默认0.5=500米</param>
        /// <returns>一维数组：0：最小经度，1：最大经度；2：最小纬度；3：最大纬度</returns>
        public static double[] FindNeighPosition(double longitude, double latitude, double aboutRadius = 0.5)
        {
            double[] result = new double[4];
            //先计算查询点的经纬度范围  
            double r = 6371;//地球半径千米  
            double dis = aboutRadius;//0.5千米距离  
            double dlng = 2 * Math.Asin(Math.Sin(dis / (2 * r)) / Math.Cos(latitude * Math.PI / 180));
            dlng = dlng * 180 / Math.PI;//角度转为弧度  
            double dlat = dis / r;
            dlat = dlat * 180 / Math.PI;
            double minlat = latitude - dlat;
            double maxlat = latitude + dlat;
            double minlng = longitude - dlng;
            double maxlng = longitude + dlng;
            result[0] = minlng;
            result[1] = maxlng;
            result[2] = minlat;
            result[3] = maxlat;
            return result;
        }
        /// <summary>
        /// 获取文件名字，不带扩展名
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetFileNameWithoutExtension(string fileName)
        {
            int length = fileName.Length - 1, dotPos = fileName.IndexOf(".");

            if (dotPos == -1)
                return fileName;

            return fileName.Substring(0, dotPos);
        }
        /// <summary>
        /// 创建一个短的GUID
        /// </summary>
        /// <returns>字符串类型</returns>
        public static string GetShortGUId()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                i *= ((int)b + 1);
            }
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }

        /// <summary>
        /// 判断文件名是否为浏览器可以直接显示的图片文件名
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>是否可以直接显示</returns>
        public static bool IsImgFilename(string filename)
        {
            filename = filename.Trim();
            if (filename.EndsWith(".") || filename.IndexOf(".") == -1)
                return false;

            string extname = filename.Substring(filename.LastIndexOf(".") + 1).ToLower();
            return (extname == "jpg" || extname == "jpeg" || extname == "png" || extname == "bmp" || extname == "gif");
        }

        /// <summary>
        /// 获取HTML里面首张图片
        /// </summary>
        /// <param name="html">html内容</param>
        /// <returns>首张图片地址，为空则图片不存在</returns>
        public static string GetHTML1stImgSrc(string html)
        {
            string imgsrc = "";
            Regex reg = new Regex("IMG[^>]*?src\\s*=\\s*(?:\"(?<1>[^\"]*)\"|'(?<1>[^\']*)')", RegexOptions.IgnoreCase);
            MatchCollection m = reg.Matches(html);
            if (m.Count > 0)
            {
                for (int i = 0; i < m.Count; i++)
                {
                    if (!string.IsNullOrEmpty(m[i].Groups[1].Value) && IsImgFilename(m[i].Groups[1].Value.ToString()))
                    {
                        imgsrc = m[i].Groups[1].Value.ToString();
                        break;
                    }
                }
            }
            else
            {
                imgsrc = "";
            }
            return imgsrc;

        }
        /// <summary>
        /// 获取所有图片
        /// </summary>
        /// <param name="pic">图片字符串</param>
        /// <returns>图片地址</returns>
        public static List<string> GetAllPic(string pic)
        {
            if (!string.IsNullOrEmpty(pic))
            {
                List<string> list = new List<string>();
                string[] arrpic = pic.Split(new string[] { "|||" }, StringSplitOptions.None);
                foreach (string picurl in arrpic)
                    list.Add(picurl);
                return list;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 从字符串的指定位置截取指定长度的子字符串
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <param name="startIndex">子字符串的起始位置</param>
        /// <param name="length">子字符串的长度</param>
        /// <returns>子字符串</returns>
        public static string CutString(string str, int startIndex, int length)
        {
            if (startIndex >= 0)
            {
                if (length < 0)
                {
                    length = length * -1;
                    if (startIndex - length < 0)
                    {
                        length = startIndex;
                        startIndex = 0;
                    }
                    else
                        startIndex = startIndex - length;
                }

                if (startIndex > str.Length)
                    return "";
            }
            else
            {
                if (length < 0)
                    return "";
                else
                {
                    if (length + startIndex > 0)
                    {
                        length = length + startIndex;
                        startIndex = 0;
                    }
                    else
                        return "";
                }
            }

            if (str.Length - startIndex < length)
                length = str.Length - startIndex;

            return str.Substring(startIndex, length);
        }
        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="strInput">字符串</param>
        /// <param name="intLen">截取长度</param>
        /// <returns></returns>
        public static string CutString(string strInput, int intLen)
        {
            if (string.IsNullOrEmpty(strInput))
                return "";
            strInput = strInput.Trim();
            byte[] myByte = Encoding.UTF8.GetBytes(strInput);
            //Response.Write("cutString Function is::" + myByte.Length.ToString());
            if (myByte.Length > intLen + 2)
            {
                //截取操作
                string resultStr = "";
                for (int i = 0; i < strInput.Length; i++)
                {
                    byte[] tempByte = Encoding.UTF8.GetBytes(resultStr);
                    if (tempByte.Length < intLen)
                    {

                        resultStr += strInput.Substring(i, 1);
                    }
                    else
                    {
                        break;
                    }
                }
                return resultStr + "...";
            }
            else
            {
                return strInput;
            }
        }
        /// <summary>
        /// 判断是否是GUID
        /// </summary>
        /// <param name="strSrc"></param>
        /// <returns></returns>
        public static bool IsGuidByParse(string strSrc)
        {
            Guid g = Guid.Empty;
            return Guid.TryParse(strSrc, out g);
        }

        /// <summary>
        /// 获取广告类型
        /// </summary>
        /// <param name="tid"></param>
        /// <returns></returns>
        public static string GetAdsType(int tid)
        {
            string tname = "代码广告";
            switch (tid)
            {
                case 0:
                    break;
                case 1:
                    tname = "文字广告";
                    break;
                case 2:
                    tname = "图片广告";
                    break;
                case 3:
                    tname = "Flash广告";
                    break;
                case 4:
                    tname = "幻灯片广告";
                    break;
                case 5:
                    tname = "漂浮广告";
                    break;
                case 6:
                    tname = "对联广告";
                    break;
            }
            return tname;
        }
        /// <summary>
        /// 获取缩略图地址
        /// </summary>
        /// <param name="imgsrc"></param>
        /// <returns></returns>
        public static string GetThumbImg(string imgsrc)
        {
            if (!string.IsNullOrEmpty(imgsrc))
            {
                return imgsrc.Replace("/userfiles/", "/userfiles/_thumbs/");
            }
            else
                return "";
        }

        /// <summary>
        /// 获取用户IP地址
        /// </summary>
        /// <returns>用户IP地址</returns>
        public static string GetIP()
        {
            string userHostAddress = HttpContextHelper.Current?.Request?.HttpContext?.Connection?.RemoteIpAddress?.ToString();
            if (!string.IsNullOrEmpty(userHostAddress) && CommonUtils.IsIP(userHostAddress))
            {
                if (userHostAddress == "::1")
                    userHostAddress = "127.0.0.1";
                return userHostAddress;
            }
            return "127.0.0.1";
        }

        /// <summary>
        /// decimal数据类型格式化
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static decimal decimalFormat(decimal data)
        {
            //分割小数点
            string[] dataArr = data.ToString().Split(".");
            string privious = dataArr[0];
            if (decimal.Parse(dataArr[1]) > 0)
            {
                var temp = dataArr[1].ToString().TrimEnd('0');
                privious = privious + "." + temp;
            }
          
            return decimal.Parse(privious);
        }
        /// <summary>
        /// 时间戳转换成DateTime格式
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static DateTime StampToDateTime(string timeStamp)
        {
            //TimeZoneInfo.ConvertTimeFromUtc()

            DateTime dateTimeStart = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), TimeZoneInfo.Local);
            long lTime = long.Parse(timeStamp + "0000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dateTimeStart.Add(toNow);
        }

        /// <summary>  
        /// 获取时间戳  
        /// </summary>  
        /// <returns></returns>  
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
        #endregion

        #region 处理部分
        /// <summary>
        /// 去除html class, style, 指定video 宽高
        /// </summary>
        /// <param name="fileName">指定文件名</param>
        /// <returns>扩展名(.xxx)</returns>
        public static async Task<string> ProcessHtml(string content,string prefixUrl, CustomHttpClientFactory _clientFactory)
        {
            MatchCollection classList = Regex.Matches(content, @"class=(""|')(?<value>.{1,100}?)(""|')");
            MatchCollection styleList = Regex.Matches(content, @"style=(""|')(?<value>.{1,100}?)(""|')");
            MatchCollection videoList = Regex.Matches(content, @"<video(.{0,300}?)></video>");

            MatchCollection eofimgsList = Regex.Matches(content, @"<img.*?src=[""'](.*?)[""'].*?>");
            foreach (var item in videoList)
            {
                var videoSrc = Regex.Match(item.ToString(), @"<video(.{0,100}?)src=(""|')(?<video>.{1,200}?)(""|')").Groups["video"].Value;
                content = content.Replace(item.ToString(), $@"<video src=""{videoSrc}"" style=""width:750px;""></video>");
            }
            foreach(System.Text.RegularExpressions.Match item in eofimgsList)
            {
                if (item.Groups.Count >= 2)
                {
                    string url = item.Groups[1].Value;
                    if (url != null)
                    {
                        if(url.IndexOf("file:///") == 0)
                        {
                            content = content.Replace($"{url}", $"{prefixUrl}/news/eof.jpg");
                        }
                        else
                        {
                            Hashtable downloadRes = await CommonUtils.GetDownload<byte[]>(_clientFactory, url);
                            if ("success" == downloadRes["status"].ToString())
                            {
                                var imagebase64 = $"data:image/" + System.IO.Path.GetExtension(new System.Uri(url).LocalPath) + ";base64," + Convert.ToBase64String((byte[])downloadRes["content"]);
                                content = content.Replace($"{url}", imagebase64);
                            }
                            else
                            {
                                content = content.Replace($"{url}", $"{prefixUrl}/news/eof.jpg");
                            }
                        }
                    }
                }
            }
            foreach (var item in classList)
            {
                content = content.Replace(item.ToString(), string.Empty);
            }

            foreach (var item in styleList)
            {
                content = content.Replace(item.ToString(), string.Empty);
            }

            return content;
        } 
        /// <summary>
        /// 图片处理
        /// </summary>
        /// <param name="imagebase64"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Bitmap ImageProcess(string imagebase64, Size size)
        {
            MemoryStream ms = new MemoryStream(Convert.FromBase64String(imagebase64));
            Image image = Image.FromStream(ms);
            //获取图片宽度
            int sourceWidth = image.Width;
            //获取图片高度
            int sourceHeight = image.Height;

            float nPercent = 1;
            float nPercentW = 0;
            float nPercentH = 0;
            if (size.Width !=0 && size.Height!=0)
            {
                //计算宽度的缩放比例
                nPercentW = ((float)size.Width / (float)sourceWidth);
                //计算高度的缩放比例
                nPercentH = ((float)size.Height / (float)sourceHeight);

                if (nPercentH < nPercentW)
                    nPercent = nPercentH;
                else
                    nPercent = nPercentW;

            }
           
            //期望的宽度
            int destWidth = (int)(sourceWidth * nPercent);
            //期望的高度
            int destHeight = (int)(sourceHeight * nPercent);

            //颜色矩阵  
            float[][] matrixItems =
            {
               new float[]{1,0,0,0,0},
               new float[]{0,1,0,0,0},
               new float[]{0,0,1,0,0},
               new float[]{0,0,0,100/255f,0},
               new float[]{0,0,0,0,1}
           };
            ColorMatrix colorMatrix = new ColorMatrix(matrixItems);
            ImageAttributes imageAtt = new ImageAttributes();
            imageAtt.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            Bitmap b = new Bitmap(destWidth, destHeight);


            Graphics g = Graphics.FromImage(b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //绘制图像
            g.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
                   0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAtt);
            g.Dispose();
            return b;
        }
        


        /// <summary>
        /// 获取标准base64字符串格式图片
        /// </summary>
        /// <param name="imageBase64">带有data:image/png;base64的base64字符串</param>

        public static string StandardBase64(string imageBase64)
        {
            return imageBase64.Split(",")[1].ToString();
        }
        /// <summary>
        /// 后台发送POST请求
        /// </summary>
        /// <param name="url">服务器地址</param>
        /// <param name="data">发送的数据</param>
        /// <returns></returns>
        public static async Task<string> PostFromDataAsync(HttpClient httpClient, string url, List<KeyValuePair<string, string>> datadic, Dictionary<string, string> headdic = null)
        {
            var content = new FormUrlEncodedContent(datadic);
            AddOrUpdHttpHeaders(content.Headers, "Content-Type", "application/x-www-form-urlencoded");
            FillHeader(content.Headers, headdic);
            var msg = await httpClient.PostAsync(url, content);
            CheckResponse(msg, url, datadic, headdic);
            var result = await msg.Content.ReadAsStringAsync();
            return result;
        }
        private static void CheckResponse(HttpResponseMessage responseMessage, string url, object data, Dictionary<string, string> headdic = null)
        {
            if (responseMessage.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new HttpRequestException($"http StatusCode:{responseMessage.StatusCode},url:{url},data:{JsonConvert.SerializeObject(data)}, headdic:{JsonConvert.SerializeObject(headdic)}");
            }
        }
        private static void FillHeader(HttpHeaders httpHeaders, Dictionary<string, string> headdic)
        {
            if (headdic != null)
            {
                foreach (var item in headdic)
                {
                    AddOrUpdHttpHeaders(httpHeaders, item.Key, item.Value);
                }
            }
        }
        private static void AddOrUpdHttpHeaders(HttpHeaders httpHeaders, string key, string value)
        {
            if (httpHeaders.Contains(key))
            {
                httpHeaders.Remove(key);
            }
            httpHeaders.Add(key, value);
        }
        
        /// <summary>
        /// 将DataTable数据导入到excel中
        /// </summary>
        /// <param name="entitys">要导入的数据</param>
        /// <param name="isColumnWritten">DataTable的列名是否要导入</param>
        /// <param name="sheetName">要导入的excel的sheet的名称</param>
        /// <param name="fileName">文件夹路径</param>
        /// <param name="title">列名数组</param>
        /// <param name="widthTable">定义列宽</param>
        /// <returns>导入数据行数(包含列名那一行)</returns>

        public static byte[] OutputExcel(IList entitys, string sheetName, string[] title, Hashtable widthTable)
        {
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet(sheetName);
            if (widthTable != null)
            {
                foreach (var k in widthTable.Keys)
                {
                    sheet.SetColumnWidth(k.ToInt(), widthTable[k].ToInt());
                }
            }
            IRow Title = null;
            IRow rows = null;
            Type entityType = entitys[0].GetType();
            PropertyInfo[] entityProperties = entityType.GetProperties();

            for (int i = 0; i <= entitys.Count; i++)
            {
                if (i == 0)
                {
                    Title = sheet.CreateRow(0);
                    for (int k = 1; k < title.Length + 1; k++)
                    {
                        Title.CreateCell(0).SetCellValue("序号");
                        
                        Title.CreateCell(k).SetCellValue(title[k - 1]);
                    }
                }
                else
                {
                    rows = sheet.CreateRow(i);
                    object entity = entitys[i - 1];
                    for (int j = 1; j <= entityProperties.Length; j++)
                    {
                        object[] entityValues = new object[entityProperties.Length];
                        entityValues[j - 1] = entityProperties[j - 1].GetValue(entity);
                        rows.CreateCell(0).SetCellValue(i);
                        rows.CreateCell(j).SetCellValue(entityValues[j - 1].ToString());
                    }
                }
            }

            byte[] buffer = new byte[1024 * 2];
            using (MemoryStream ms = new MemoryStream())
            {
                workbook.Write(ms);
                buffer = ms.GetBuffer();
                ms.Close();
            }
            return buffer;
        }

        //常用处理函数
        /// <summary>
        /// 格式化处理SQL字符串
        /// </summary>
        /// <param name="str">替换单引号后的字符串</param>
        /// <returns>格式化后的SQL字符串</returns>
        public static string SqlStr(string str)
        {
            string re = "";
            try
            {
                re = str.Replace("'", "''");
            }
            catch
            {
                re = "";
            }
            return re;
        }
        /// <summary>
        ///一个处理危险HTML标签的方法
        /// </summary>
        /// <param name="M_Htmlstring">要处理的字符串</param>
        /// <returns></returns>
        public static string NoHTML(string M_Htmlstring) //去除HTML标记 
        {
            if (string.IsNullOrWhiteSpace(M_Htmlstring))
                return "";
            //删除脚本 
            M_Htmlstring = Regex.Replace(M_Htmlstring, @"<script[^>]*?>.*? </script>", string.Empty, RegexOptions.IgnoreCase);
            //删除HTML 
            M_Htmlstring = Regex.Replace(M_Htmlstring, @"<(.?[^>]*)>", string.Empty, RegexOptions.IgnoreCase);
            M_Htmlstring = Regex.Replace(M_Htmlstring, @"([\r\n])[\s]+", string.Empty, RegexOptions.IgnoreCase);//空格换行
            M_Htmlstring = Regex.Replace(M_Htmlstring, @"-->", string.Empty, RegexOptions.IgnoreCase);//注释
            M_Htmlstring = Regex.Replace(M_Htmlstring, @" <!--.*", string.Empty, RegexOptions.IgnoreCase);
            M_Htmlstring = Regex.Replace(M_Htmlstring, @"&(quot|34);", "\"", RegexOptions.IgnoreCase);
            M_Htmlstring = Regex.Replace(M_Htmlstring, @"&(amp|38);", "&", RegexOptions.IgnoreCase);
            M_Htmlstring = Regex.Replace(M_Htmlstring, @"&(lt|60);", " <", RegexOptions.IgnoreCase);
            M_Htmlstring = Regex.Replace(M_Htmlstring, @"&(gt|62);", ">", RegexOptions.IgnoreCase);
            M_Htmlstring = Regex.Replace(M_Htmlstring, @"&(nbsp|160);", " ", RegexOptions.IgnoreCase);
            M_Htmlstring = Regex.Replace(M_Htmlstring, @"&(iexcl|161);", "\xa1", RegexOptions.IgnoreCase);
            M_Htmlstring = Regex.Replace(M_Htmlstring, @"&(cent|162);", "\xa2", RegexOptions.IgnoreCase);
            M_Htmlstring = Regex.Replace(M_Htmlstring, @"&(pound|163);", "\xa3", RegexOptions.IgnoreCase);
            M_Htmlstring = Regex.Replace(M_Htmlstring, @"&(copy|169);", "\xa9", RegexOptions.IgnoreCase);
            M_Htmlstring = Regex.Replace(M_Htmlstring, @" href *= *[\s\S]*script *:", string.Empty, RegexOptions.IgnoreCase);
            M_Htmlstring = Regex.Replace(M_Htmlstring, @" on[\s\S]*=", string.Empty, RegexOptions.IgnoreCase);//过滤其它控件的on...事件
            M_Htmlstring = Regex.Replace(M_Htmlstring, @"<iframe[\s\S]+</iframe *>", string.Empty, RegexOptions.IgnoreCase);//过滤iframe
            M_Htmlstring = Regex.Replace(M_Htmlstring, @"<frameset[\s\S]+</frameset *>", string.Empty, RegexOptions.IgnoreCase);//过滤frameset
            M_Htmlstring = Regex.Replace(M_Htmlstring, @"<", string.Empty, RegexOptions.IgnoreCase);
            M_Htmlstring = Regex.Replace(M_Htmlstring, @">", string.Empty, RegexOptions.IgnoreCase);
            // M_Htmlstring.Replace("\r\n", string.Empty);
            M_Htmlstring = M_Htmlstring.Trim();
            return M_Htmlstring;
        }
        /// <summary>
        /// 普通过滤脚本、框架
        /// </summary>
        /// <param name="M_Html">要处理的字符串</param>
        /// <returns>去除脚本、框架、style样式标记</returns>
        public static string NoScriptAndIframe(string M_Html)
        {
            if (!string.IsNullOrEmpty(M_Html))
            {
                M_Html = Regex.Replace(M_Html, @"^<style\\s*[^>]*>([^>]|[~<])*<\\/style>$", string.Empty, RegexOptions.IgnoreCase);
                M_Html = Regex.Replace(M_Html, @"<script[^>]*?>.*? </script>", string.Empty, RegexOptions.IgnoreCase);
                M_Html = Regex.Replace(M_Html, @"<iframe[\s\S]+</iframe *>", string.Empty, RegexOptions.IgnoreCase);//过滤iframe
                M_Html = Regex.Replace(M_Html, @"<frameset[\s\S]+</frameset *>", string.Empty, RegexOptions.IgnoreCase);//过滤frameset
            }
            else
            {
                M_Html = "";
            }
            return M_Html.Trim();
        }
        /// <summary>
        /// 清除img格式
        /// </summary>
        /// <param name="M_Html">要处理的字符串</param>
        /// <returns>去除脚本、框架、style样式标记</returns>
        public static string getSimpleImg(string content)
        {
            string fullImgRegPattern = @"(?<value><img.{1,500}?(>|/>))";
            string imgRegPattern = @"(?s)<img(.{1,300}?)(\s| )src=('|"")(?<value>.{1,300}?)('|"")";
            string imgRegPattern2 = @"(?s)<img(\s| )src=('|"")(?<value>.{1,300}?)('|"")";

            List<string> fullImgUrlList = RegexHelper.GetValueList(content, fullImgRegPattern);
            if (null != fullImgUrlList)
            {
                foreach (string fullstr in fullImgUrlList)
                {
                    var url = RegexHelper.GetValue(fullstr, imgRegPattern);
                    if (string.IsNullOrEmpty(url))
                    {
                        url = RegexHelper.GetValue(fullstr, imgRegPattern2);
                    }
                    var img = string.Format(@"<img src=""{0}"" />", url);
                    content = content.Replace(fullstr, img);
                }
            }
            return content;
        }
        /// <summary>    
        /// 过滤字符串中的html代码    
        /// </summary>    
        /// <param name="Str"></param>    
        /// <returns>返回过滤之后的字符串</returns>    
        public static string LostHTML(string Str)
        {
            if (string.IsNullOrWhiteSpace(Str))
                return "";

            string Re_Str = "";
            string Pattern = "<\\/*[^<>]*>";
            Re_Str = Regex.Replace(Str, Pattern, "");
            return (Re_Str.Replace("\\r\\n", "")).Replace("\\r", "");
        }
        /// <summary>
        /// 删除最后结尾的指定字符后的字符
        /// </summary>
        public static string DelLastChar(string str, string strchar)
        {
            if (string.IsNullOrEmpty(str))
                return "";
            if (str.LastIndexOf(strchar) >= 0 && str.LastIndexOf(strchar) == str.Length - 1)
            {
                return str.Substring(0, str.LastIndexOf(strchar));
            }
            return str;
        }

        /// <summary>
        /// HTTP GET方式请求数据.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <returns></returns>
        public static string HttpGet(string url)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "GET";
            //request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "*/*";
            request.Proxy = null;
            request.Timeout = 15000;
            request.AllowAutoRedirect = false;

            WebResponse response = null;
            string responseStr = null;

            try
            {
                response = request.GetResponse();

                if (response != null)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    responseStr = reader.ReadToEnd();
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                NewLife.Log.XTrace.WriteException(ex);
                throw;
            }
            finally
            {
                request = null;
                response = null;
            }

            return responseStr;
        }
        /// <summary>
        /// HTTP GET方式请求数据.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <returns></returns>
        public static async Task<Hashtable> GetDownload<T>(CustomHttpClientFactory clientFactory,string url, string proxy = null, int timeout = 10 * 1000)
        {
            Hashtable resData = new Hashtable();
            resData.Add("status", "success");
            T result = default(T);
            try
            {
                for (int i = 0; i < 2; i++)
                {
                    //var handler = new HttpClientHandler
                    //{
                    //    UseProxy = false,
                    //    AllowAutoRedirect = true,
                    //    AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                    //    ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true
                    //};

                    //if (null != proxy && "" != proxy)
                    //{
                    //    handler.Proxy = new WebProxy(proxy, false);
                    //    handler.UseProxy = true;
                    //}

                    //using 
                    var client = clientFactory.CreateClient();
                    {
                        //client.Timeout = TimeSpan.FromMilliseconds(timeout);
                        var response = await client.GetAsync(url);
                        var contentType = response.Content.Headers.ContentType?.ToString();

                        if (false == response.IsSuccessStatusCode)
                        {
                            throw new Exception($"get {url} error({(int)response.StatusCode})");
                        }

                        if (typeof(T) == typeof(string))
                        {
                            result = (T)(object)await response.Content.ReadAsStringAsync();
                        }
                        else if (typeof(T) == typeof(byte[]))
                        {
                            result = (T)(object)await response.Content.ReadAsByteArrayAsync();

                            if (null == contentType || "" == contentType)
                            {
                                contentType = "image/jpeg";
                            }
                        }
                        else if (typeof(T) == typeof(Stream))
                        {
                            result = (T)(object)await response.Content.ReadAsStreamAsync();
                        }
                        else
                        {
                            throw new Exception($"data type not supported");
                        }
                        resData.Add("contentType", contentType);
                        resData.Add("content", result);
                    }
                    break;
                }
            }
            catch (Exception e)
            {
                 resData["status"] = $"url:{url}\r\n\r\n{e.ToString()}";
                 return resData;
            }
            return resData;
        }

      
        #endregion
    }
}
