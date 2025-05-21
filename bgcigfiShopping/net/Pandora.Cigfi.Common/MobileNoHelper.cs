using System;
using System.Text.RegularExpressions;
using PhoneNumbers;

namespace Pandora.Cigfi.Common
{
    /// <summary>
    /// 手机号相关的处理类库
    /// </summary>
    public class MobileNoHelper
    {

        /// <summary>
        /// 根据 +86***** 手机号拆分获取手机号及区号
        /// </summary>
        /// <param name="mobileNo">手机号</param>
        /// <returns></returns>
        [Obsolete]
        public static MobileNoInfo GetMobileNoModel(string mobileNo)
        {
            MobileNoInfo mobileNoInfo = new MobileNoInfo();

            mobileNo = mobileNo.Replace("+", "").Trim();
            string[] sArray = mobileNo.Split('_'); //拆分字符串
            if (sArray.Length == 1)
            {
                mobileNoInfo.AreaCode = "86"; //区号。如：86
                mobileNoInfo.Mobile = sArray[0].ToString(); //手机号码。如：13823846589
            }
            else
            {
                mobileNoInfo.AreaCode = sArray[0].ToString(); //区号。如：86
                mobileNoInfo.Mobile = sArray[1].ToString(); //手机号码。如：13823846589
            }
            return mobileNoInfo;
        }
        /// <summary>
        /// 根据手机号及区号 获取完整手机号
        /// </summary>
        public static string GetFullMobileNo(string areaCode, string mobileNo)
        {
            if (string.IsNullOrEmpty(areaCode))
            {
                return mobileNo;
            }
            else
            {
                if (areaCode.IndexOf("+") == -1)
                {
                    areaCode = "+" + areaCode;
                }
                return areaCode + mobileNo;
            }
        }
        /// <summary>
        /// 验证手机号是否有效
        /// </summary>
        public static bool IsVaildPhoneNumber(string areaCode, string mobileNo)
        {
            string pattern = @"^[0-9]*[1-9][0-9]*$";
            if ((string.IsNullOrEmpty(areaCode) && !Regex.IsMatch(areaCode, pattern)) || !Regex.IsMatch(mobileNo, pattern))
            {
                return false;
            }
            // 后来使用libphonenumber -csharp 解决了 [2019-08-21]
            PhoneNumberUtil phoneUtil = PhoneNumberUtil.GetInstance();
            var build = new PhoneNumber.Builder();
            if (!string.IsNullOrEmpty(areaCode))
            {
                build = build.SetCountryCode(Convert.ToInt32(areaCode));
            }
            var nzNumber = build.SetNationalNumber(Convert.ToUInt64(mobileNo)).Build();
            return phoneUtil.IsValidNumber(nzNumber);

        }

        /// <summary>
        /// 获取手机号对象
        /// </summary>
        public static MobileNoInfo GetMobileModel(string phone, string language = "CN")
        {
            MobileNoInfo mobileNoInfo = new MobileNoInfo();
            PhoneNumberUtil phoneUtil = PhoneNumberUtil.GetInstance();
            try
            {
                var nzNumber = phoneUtil.Parse(phone, language);
                if (phoneUtil.IsValidNumber(nzNumber))
                {
                    mobileNoInfo.AreaCode = nzNumber.CountryCode.ToString();
                    mobileNoInfo.Mobile = nzNumber.NationalNumber.ToString();
                }
            }
            catch
            {
            }
            return mobileNoInfo;

        }
    }


    /// <summary>
    /// 手机号模型
    /// </summary>
    public class MobileNoInfo
    {
        public MobileNoInfo()
        {
            AreaCode = Mobile = string.Empty;
        }
        /// <summary>
        /// 区号
        /// </summary>
        public string AreaCode
        {
            get;
            set;
        }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile
        {
            get;
            set;
        }


    }

}
