using  System;
using  System.Collections.Generic;
using   System.Linq;
using  System.Threading.Tasks;
using Pandora.Cigfi.Web.Common;
using Pandora.Cigfi.Core.MiddlewareExtension;
using Microsoft.AspNetCore.Http;

namespace Pandora.Cigfi.Web.ExceptionHandler
{
    public class ResultExceptionHandler : IExceptionHandler
    {
        public async Task ExceptionHandle(HttpContext context, Exception exception)
        {
            string message = exception.Message;

            ReJson error = new ReJson(message);
            context.Response.ContentType = "text/json";
            await context.Response.WriteAsync(error.ToJson());
        }
    }
}
