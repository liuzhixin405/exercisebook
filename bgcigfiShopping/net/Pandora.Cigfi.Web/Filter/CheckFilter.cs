﻿using  System;
using  System.Collections.Generic;
using   System.Linq;
using  System.Threading.Tasks;
using Pandora.Cigfi.Common;
using Pandora.Cigfi.Web.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Pandora.Cigfi.Web.Filter
{
    public class CheckFilterAttribute : ActionFilterAttribute
    {
        public override async void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var error = string.Empty;

                foreach (var key in context.ModelState.Keys)
                {
                    var state = context.ModelState[key];
                    if (state.Errors.Any())
                    {
                        error = string.Join("\r\n", state.Errors.ToList().ConvertAll(item => item.ErrorMessage));
                        if (error.Equals(string.Empty))
                            error = string.Join("\r\n", state.Errors.ToList().ConvertAll(item => item.Exception.Message));
                        break;
                    }
                }

                throw new Exception(error);
            }

            var notVali = ValiSignature(context.HttpContext);

            if (notVali == null)
            {
               await context.HttpContext.Response.WriteAsync(notVali.ToJson());
            }
        }

        private ReJson ValiSignature(HttpContext context)
        {
            var Request = context.Request;

            var queryStrings = Request.Query;
            List<string> keys = new List<string>();
            //string[] keys = HttpContext.Current.Request.QueryString.AllKeys;
            if (queryStrings == null)
            {
                throw new Exception("queryStrings Is Null");
            }
            foreach (var item in queryStrings.Keys)
            {
                keys.Add(item);
            }
            SortedDictionary<string, string> pars = new SortedDictionary<string, string>();
            string signature = "";
            foreach (var k in keys)
            {
                if (k != "signature")
                {
                    string v = Request.Query[k];
                    pars.Add(k, v);
                }
                else
                {
                    signature = Request.Query[k];
                }
            }
            //没有签名返回错误
            if (string.IsNullOrEmpty(signature))
                throw new Exception("signature Is Null");

            if (!MySign.CheckSign(pars, signature))
            {

                return new ReJson(40004, "signature 错误！");
            }
            //判断是否超时
            string timeStamp = pars["timeStamp"];
            //判断时间有效性
            DateTime postTime = CommonUtils.StampToDateTime(timeStamp);
            if (postTime < DateTime.Now.AddSeconds(-120))//30秒有效期
            {
                return new ReJson(40004, "signature 错误！", 1);
            }

            return null;
        }
    }
}
