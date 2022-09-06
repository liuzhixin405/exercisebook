using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Extensions
{
    /// <summary>
    /// action缓存
    /// </summary>
    public class CustomActionFilterAttribute : Attribute, IResourceFilter,IFilterMetadata,IOrderedFilter
    {
        private static Dictionary<string, IActionResult> _customCacheResourceFilterAttributeDictionary = new Dictionary<string, IActionResult>();
        public int Order => 0;

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            string key = context.HttpContext.Request.Path;
            _customCacheResourceFilterAttributeDictionary.Add(key, context.Result);
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            string key = context.HttpContext.Request.Path;
            if (_customCacheResourceFilterAttributeDictionary.ContainsKey(key))
            {
                context.Result = _customCacheResourceFilterAttributeDictionary[key];
            }
        }
    }
}
