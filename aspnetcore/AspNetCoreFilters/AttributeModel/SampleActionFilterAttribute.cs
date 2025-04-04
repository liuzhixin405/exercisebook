﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreFilters.AttributeModel
{
    public class SampleActionFilterAttribute: TypeFilterAttribute
    {
        public SampleActionFilterAttribute():base(typeof(SampleActionFilterImpl))
        {

        }
    }
    public class SampleActionFilterImpl : IActionFilter
    {
        private readonly ILogger _logger;
        public SampleActionFilterImpl(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<SampleActionFilterAttribute>();
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation("SampleActionFilterAttribute.OnActionExecuted");
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation("SampleActionFilterAttribute.OnActionExecuting");
        }
    }
}
