using Microsoft.AspNetCore.Mvc.Filters;

namespace Chat
{
    public class HttpContextMiddleware
    {
        private readonly RequestDelegate _next;
        public HttpContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            var query = context.Request.QueryString.ToString().ToLower();
            var path = context.Request.Path.ToString().ToLower();

            if (!path.Equals("/chat/getanswer")&& !query.StartsWith("?q="))
            {
                context.Response.StatusCode = 200;
                context.Response.ContentType= "text/plain;charset=utf-8";
               
                await context.Response.WriteAsync("你还没问问题呢。比如这样问:http://www.eiza.net/chat?q=今天天气如何?");
            }
            else
            {
                await _next(context);
            }
        }
    }
}
