using System;
using System.Collections.Generic;
using System.Text;

namespace Pandora.Cigfi.Models.Consts
{
    /// <summary>
    /// 事件按钮常量
    /// </summary>
    public class ResponseCode
    {
        /// <summary>
        ///  OK
        /// </summary>
        public const int OK = 200;
        /// <summary>
        ///  出错，允许自定义错误提示
        /// </summary>
        public const int ERROR = 400;
        /// <summary>
        ///  鉴权失败
        /// </summary>
        public const int AUTHFAIL = 401;
        /// <summary>
        ///  鉴权成功但是用户无权访问
        /// </summary>
        public const int AUTHSUC_NORIGHT = 403;
        /// <summary>
        ///  找不到
        /// </summary>
        public const int NOTFOUND = 404;
        /// <summary>
        ///  给出的参数超出范围
        /// </summary>
        public const int PARAM_ERROR = 416;
        /// <summary>
        ///  服务器内部出错
        /// </summary>
        public const int SERVER_ERROR = 500;
        /// <summary>
        /// 已存在该手机号码用户
        /// </summary>
        public const int EXISTSPHONE = 400;

        /// <summary>
        ///  密码错误
        /// </summary>
        public const int PWD_ERROR = 1303;
        /// <summary>
        ///  用户不存在
        /// </summary>
        public const int USER_NOTFOUND = 1206;

        /// <summary>
        ///  无效token
        /// </summary>
        public const int INVALIDSIGNATURE = 13;
        /// <summary>
        ///  密码错误
        /// </summary>
        public const int CHECKPWD_USER_PWD_ERROR = 1303;
        /// <summary>
        ///  用户不存在
        /// </summary>
        public const int CHECKPWD_USER_NOTFOUND = 2305;
        /// <summary>
        ///  短信次数超限
        /// </summary>
        public const int SMS_FREQUENT= 2307;
        /// <summary>
        /// 验证码错误
        /// </summary>
        public const int SMSCODEERROR = 1003;    
        /// <summary>
        /// 已存在该手机号码用户
        /// </summary>
        public const int CHANGEMOBILE_EXISTSPHONE = 1103;
        /// <summary>
        /// 入驻信息已经填写过
        /// </summary>
        public static int SETTLED_EXISTS = 409;

        /// <summary>
        /// 含有敏感词汇，
        /// </summary>
        public static int SMS_SENSITIVEWORD = 2400;
        /// <summary>
        /// 标题太短
        /// </summary>
        public static int TITLE_TOOSHORT=1416;

        /// <summary>
        ///  给出的参数超出范围
        /// </summary>
        public const int PARAM_ERROR_LONGTIPS = 1416;
        /// <summary>
        /// 草稿箱已满
        /// </summary>
        public static int DRAFT_BOX_FULL= 1413;
    }
}
