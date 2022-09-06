﻿using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreFilters.AttributeModel
{
    /// <summary>
    /// 返回结果前头部增加
    /// </summary>
    public class AddHeaderAttribute:ResultFilterAttribute
    {
        private readonly string _name;
        private readonly string _value;

        public AddHeaderAttribute(string name, string @value)
        {
            _name = name;
            _value = @value;
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            context.HttpContext.Response.Headers.Add(_name, new string[] { _value });
            base.OnResultExecuting(context);
        }
    }
}
