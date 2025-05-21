/*
 * 作者：pc/DESKTOP-QGGMQC1
 * 时间：2018-09-13 10:47:53
 * 版权：版权所有 (C) 小号科技 研发团队 2017~2018
*/
using Pandora.Cigfi.Models.Consts;
using Pandora.Cigfi.Models.ResponseMessage;
using Pandora.Cigfi.Common;

namespace Pandora.Cigfi.Models.Sms
{

    public class SmsCodeCheckModel
    {
        /// <summary>
        /// 国家区号
        /// </summary>
        public string AreaCode { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        public string Sms_Code { get; set; }

        /// <summary>
        /// 短信类型
        /// </summary>
        public SmsCodeCheckType Type { get; set; }
        /// <summary>
        /// Token
        /// </summary>
        public string Token { get; set; }
        public ResponseMessage<string> Check(string lan)
        {
            var message = new ResponseMessage<string>(); // Fix for IDE0090: Simplify "new" expression
            if (string.IsNullOrWhiteSpace(AreaCode) || string.IsNullOrWhiteSpace(Mobile) || string.IsNullOrWhiteSpace(Sms_Code))
            {
                message.Code = ResponseCode.PARAM_ERROR;
                message.Msg = "参数错误"; // Fix for CS0234: Replace ResponseMsg.PARAM_ERROR with a string literal
                return message;
            }
            if (Type == SmsCodeCheckType.ChangePassword && string.IsNullOrEmpty(Token))
            {
                message.Code = ResponseCode.PARAM_ERROR;
                message.Msg = "参数错误"; // Fix for CS0234: Replace ResponseMsg.PARAM_ERROR with a string literal
                return message;
            }
            if (AreaCode.Contains("+")) // Fix for CA2249: Use string.Contains instead of string.IndexOf
            {
                AreaCode = AreaCode.Replace("+", "");
            }
            if (!MobileNoHelper.IsVaildPhoneNumber(AreaCode, Mobile))
            {
                message.Code = ResponseCode.PARAM_ERROR;
                message.Msg = "手机号错误"; // Fix for CS0234: Replace ResponseMsg.PHONEERROR with a string literal
                return message;
            }
            message.Code = ResponseCode.OK;
            message.Msg = "成功"; // Fix for CS0234: Replace ResponseMsg.SUCCESS with a string literal
            return message;
        }
    }
    public enum SmsCodeCheckType
    {
        /// <summary>
        /// 手机注册
        /// </summary>
        Reg = 1,
        /// <summary>
        /// 忘记密码
        /// </summary>
        FindPassword = 2,
        /// <summary>
        /// 修改密码
        /// </summary>
        ChangePassword = 3,
        /// <summary>
        /// 手机登录
        /// </summary>
        Login = 5
    }
}