﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SignService.Filters;
using SignService.Util.Primitives;
using System;
using System.Text;

namespace SignService.Controllers
{
    /// <summary>
    /// 基控制器
    /// </summary>
    [FormatResponse]
    public class BaseController : ControllerBase
    {

        /// <summary>
        /// 返回JSON
        /// </summary>
        /// <param name="jsonStr">json字符串</param>
        /// <returns></returns>
        protected ContentResult JsonContent(string jsonStr)
        {
            return base.Content(jsonStr, "application/json", Encoding.UTF8);
        }

        /// <summary>
        /// 返回html
        /// </summary>
        /// <param name="body">html内容</param>
        /// <returns></returns>
        protected ContentResult HtmlContent(string body)
        {
            return base.Content(body);
        }

        /// <summary>
        /// 返回成功
        /// </summary>
        /// <returns></returns>
        protected AjaxResult Success()
        {
            AjaxResult res = new AjaxResult
            {
                Success = true,
                Msg = "请求成功！",
            };

            return res;
        }

        /// <summary>
        /// 返回成功
        /// </summary>
        /// <param name="data">返回数据</param>
        /// <returns></returns>
        protected AjaxResult<T> Success<T>(T data)
        {
            AjaxResult<T> res = new AjaxResult<T>
            {
                Success = true,
                Msg = "操作成功",
                Data = data
            };

            return res;
        }

        /// <summary>
        /// 返回成功
        /// </summary>
        /// <param name="data">返回数据</param>
        /// <param name="msg">返回消息</param>
        /// <returns></returns>
        protected AjaxResult<T> Success<T>(T data, string msg)
        {
            AjaxResult<T> res = new AjaxResult<T>
            {
                Success = true,
                Msg = msg,
                Data = data
            };

            return res;
        }

        /// <summary>
        /// 返回错误
        /// </summary>
        /// <returns></returns>
        protected AjaxResult Error()
        {
            AjaxResult res = new AjaxResult
            {
                Success = false,
                Msg = "请求失败！",
            };

            return res;
        }

        /// <summary>
        /// 返回错误
        /// </summary>
        /// <param name="msg">错误提示</param>
        /// <returns></returns>
        protected AjaxResult Error(string msg)
        {
            AjaxResult res = new AjaxResult
            {
                Success = false,
                Msg = msg,
            };

            return res;
        }
    }
}
